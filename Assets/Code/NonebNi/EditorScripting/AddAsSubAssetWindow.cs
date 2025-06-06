﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NonebNi.EditorScripting
{
    /// <summary>
    /// Copy pasted from https://github.com/mob-sakai/SubAssetEditor/blob/main/Editor/SubAssetEditor.cs
    /// It's small enough of a thing that I want to maintain personally without adding "yet another package", and I don't
    /// really want to write from scratch so here we go.
    /// </summary>
    internal class SubAssetEditor : EditorWindow
    {
        private const string k_IconGuidLight = "b6e0adfc737ab4640967690d0b3fddfa";
        private const string k_IconGuidDark = "2665db9ef44f04f4a9013f5a40a8fa73";

        [SerializeField] private Object m_MainAsset;

        [SerializeField] private bool m_ShowAddableOnly = true;

        private readonly HashSet<Object> _addableAssets = new();
        private readonly List<Object> _dependentAssets = new();
        private readonly List<Object> _subAssets = new();
        private GUIContent _appendContent;
        private GUIContent _dependentContent;
        private int _dirtyCount;
        private GUIContent _dropAreaContent;
        private bool _isDirty;
        private GUIContent _removeContent;
        private Vector2 _scrollPosition;
        private GUIContent _visibleOffContent;
        private GUIContent _visibleOnContent;

        public bool isInGUI { get; private set; }

        private void OnEnable()
        {
            Postprocessor.s_Editor = this;
            var iconGuid = EditorGUIUtility.isProSkin ?
                k_IconGuidDark :
                k_IconGuidLight;
            var icon = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(iconGuid));
            titleContent = new GUIContent("Sub Assets", icon);
            Refresh();
        }

        private void OnDisable()
        {
            Postprocessor.s_Editor = null;
        }

        private void OnGUI()
        {
            isInGUI = true;
            InitializeIfNeeded();

            EditorGUI.BeginChangeCheck();
            m_MainAsset = EditorGUILayout.ObjectField("Main Asset", m_MainAsset, typeof(Object), false);
            if (EditorGUI.EndChangeCheck()) Refresh();

            if (!m_MainAsset)
            {
                EditorGUILayout.HelpBox("Select a main asset to edit sub assets.", MessageType.Info);
                return;
            }

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            // Header: Sub Assets
            EditorGUILayout.Space();
            var pTitle = EditorGUILayout.GetControlRect(false);
            EditorGUI.LabelField(pTitle, "Sub Assets", EditorStyles.boldLabel);

            if (0 == _subAssets.Count) GUILayout.Label(" <i>No sub assets</i>", "ShurikenEditableLabel");

            foreach (var subAsset in _subAssets)
            {
                if (!subAsset) continue;

                // Thumbnail
                var visible = 0 == (subAsset.hideFlags & HideFlags.HideInHierarchy);
                GUI.color = new Color(
                    1,
                    1,
                    1,
                    visible ?
                        1 :
                        0.5f
                );
                var p = EditorGUILayout.GetControlRect(false);
                var thumbnail = EditorGUIUtility.TrTempContent("");
                thumbnail.image = AssetPreview.GetMiniThumbnail(subAsset);
                var p1 = new Rect(p.x + 20, p.y, 18, 18);
                if (GUI.Button(p1, thumbnail, "IconButton")) EditorGUIUtility.PingObject(subAsset);

                // Rename
                var assetName = subAsset.name;
                var p2 = new Rect(p.x + 40, p.y, p.width - 102, p.height);
                var newName = EditorGUI.DelayedTextField(p2, assetName);
                if (newName != assetName)
                {
                    subAsset.name = newName;
                    _isDirty = true;
                }

                // Dependent
                GUI.color = Color.white;
                if (_dependentAssets.Contains(subAsset))
                {
                    var p3 = new Rect(p.x + p.width - 60, p.y, 20, 20);
                    EditorGUI.LabelField(p3, _dependentContent);
                }

                // Visible
                var p4 = new Rect(p.x + p.width - 40, p.y, 18, 18);
                if (GUI.Button(
                        p4,
                        visible ?
                            _visibleOnContent :
                            _visibleOffContent,
                        "IconButton"
                    ))
                {
                    subAsset.hideFlags ^= HideFlags.HideInHierarchy;
                    _isDirty = true;
                }

                // Remove
                var p5 = new Rect(p.x + p.width - 20, p.y, 18, 18);
                if (GUI.Button(p5, _removeContent, "IconButton")
                    && EditorUtility.DisplayDialog(
                        "Remove Sub Asset",
                        $"Remove '{subAsset.name}' from '{m_MainAsset.name}'?",
                        "Remove",
                        "Cancel"
                    ))
                {
                    DestroyImmediate(subAsset, true);
                    _isDirty = true;
                    GUIUtility.hotControl = 0;
                }
            }

            // Drag and drop area to append sub assets
            var pDrop = EditorGUILayout.GetControlRect(false);
            pDrop.xMin += 20;
            if (DrawDropArea(pDrop, _dropAreaContent, out var droppedObjects))
            {
                Add(m_MainAsset, droppedObjects);
                _isDirty = true;
            }

            // Dependent Assets
            if (0 < _dependentAssets.Count)
            {
                // Header: Dependent Assets
                EditorGUILayout.Space();
                var p = EditorGUILayout.GetControlRect(false);
                EditorGUI.LabelField(new Rect(p.x, p.y, 140, 18), "Dependent Assets", EditorStyles.boldLabel);
                m_ShowAddableOnly = GUI.Toggle(new Rect(140, p.y, 200, 18), m_ShowAddableOnly, "Show Addable Only");

                foreach (var asset in _dependentAssets)
                {
                    if (!asset || _subAssets.Contains(asset)) continue;

                    var appendable = _addableAssets.Contains(asset);
                    if (m_ShowAddableOnly && !appendable) continue;

                    // Object
                    p = EditorGUILayout.GetControlRect(false);
                    var p2 = new Rect(p.x + 20, p.y, p.width - 42, p.height);
                    EditorGUI.ObjectField(p2, asset, typeof(Object), false);

                    // Add
                    var p3 = new Rect(p.x + p.width - 20, p.y, 18, 18);
                    if (appendable
                        && GUI.Button(p3, _appendContent, "IconButton")
                        && EditorUtility.DisplayDialog(
                            "Add Sub Asset",
                            $"Add '{asset.name}' to '{m_MainAsset.name}'?",
                            "Add",
                            "Cancel"
                        ))
                    {
                        Add(m_MainAsset, asset);
                        _isDirty = true;
                    }
                }
            }

            EditorGUILayout.EndScrollView();

            if (_isDirty)
            {
                _isDirty = false;
                EditorUtility.SetDirty(m_MainAsset);
                AssetDatabase.SaveAssets();
                Refresh();
            }
            else if (_dirtyCount < EditorUtility.GetDirtyCount(m_MainAsset))
            {
                Refresh();
            }

            isInGUI = false;
        }

        [MenuItem("NonebNi/Edit Sub Assets")]
        private static void Open()
        {
            var editor = GetWindow<SubAssetEditor>();

            var current = Selection.activeObject;
            if (current && AssetDatabase.Contains(current))
            {
                editor.m_MainAsset = AssetDatabase.IsMainAsset(current) ?
                    current :
                    AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(current));
                editor.Refresh();
            }
        }

        private void InitializeIfNeeded()
        {
            if (_visibleOnContent != null) return;

            _visibleOnContent = EditorGUIUtility.TrTextContentWithIcon(
                "",
                "Toggle visibility in project view",
                "scenevis_visible_hover"
            );
            _visibleOffContent =
                EditorGUIUtility.TrTextContentWithIcon("", "Toggle visibility in project view", "sceneviewvisibility");
            _dependentContent =
                EditorGUIUtility.TrTextContentWithIcon("", "Dependent Asset", "unityeditor.finddependencies");
            _appendContent = EditorGUIUtility.TrTextContentWithIcon("", "Add to main asset", "toolbar plus");
            _removeContent = EditorGUIUtility.TrTextContentWithIcon("", "Remove from main asset", "toolbar minus");
            _dropAreaContent =
                EditorGUIUtility.TrTextContentWithIcon("Drag & drop assets to append", "", "toolbar plus");
        }

        private void Refresh()
        {
            _subAssets.Clear();
            _dependentAssets.Clear();
            _addableAssets.Clear();

            if (!m_MainAsset || !AssetDatabase.IsMainAsset(m_MainAsset))
            {
                m_MainAsset = null;
                return;
            }

            _dirtyCount = EditorUtility.GetDirtyCount(m_MainAsset);

            // Find sub assets.
            var allAssetsInTarget = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(m_MainAsset));
            _subAssets.AddRange(
                allAssetsInTarget
                    .Where(x => x != m_MainAsset && !(x is Component) && !(x is GameObject))
            );

            // Find dependent assets.
            foreach (var subAsset in allAssetsInTarget)
            {
                var sp = new SerializedObject(subAsset).GetIterator();
                sp.Next(true);

                // Find dependent assets in SerializedProperties.
                while (sp.Next(true))
                {
                    if (sp.propertyType != SerializedPropertyType.ObjectReference || !sp.objectReferenceValue) continue;
                    var refObject = sp.objectReferenceValue;
                    if (refObject != m_MainAsset && !_dependentAssets.Contains(refObject))
                    {
                        _dependentAssets.Add(refObject);
                        if (IsAddable(refObject, m_MainAsset)) _addableAssets.Add(refObject);
                    }
                }
            }
        }

        private static bool DrawDropArea(Rect pos, GUIContent content, out Object[] droppedObjects)
        {
            droppedObjects = null;
            GUI.Box(pos, content, EditorStyles.helpBox);

            var id = GUIUtility.GetControlID(FocusType.Passive);
            var evt = Event.current;
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!pos.Contains(evt.mousePosition)) break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    DragAndDrop.activeControlID = id;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        DragAndDrop.activeControlID = 0;
                        droppedObjects = DragAndDrop.objectReferences;
                    }

                    Event.current.Use();
                    break;
            }

            return droppedObjects != null;
        }

        private static void Add(Object mainAsset, params Object[] assets)
        {
            // Instantiate sub-assets.
            var assetsToAdd = assets
                .Where(x => IsAddable(x, mainAsset))
                .Select(
                    origin =>
                    {
                        // Not-readable texture check.
                        if (origin is Texture t && !t.isReadable)
                        {
                            Debug.LogError(
                                $"Texture '{origin.name}' is not readable. Please change the texture importer to readable.",
                                origin
                            );
                            return (null, null);
                        }

                        var newInstance = Instantiate(origin);
                        newInstance.name = origin.name;
                        return (origin, newInstance);
                    }
                )
                .Where(x => x.origin && x.newInstance)
                .ToArray();

            // Add sub-assets.
            Array.ForEach(assetsToAdd, x => AssetDatabase.AddObjectToAsset(x.newInstance, mainAsset));
            AssetDatabase.SaveAssets();

            // Get all SerializedObjects.
            var serializedObjects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(mainAsset))
                .Select(x => new SerializedObject(x))
                .ToArray();

            // Replace dependent assets to new instance.
            foreach (var x in assetsToAdd)
                // Find dependent assets in SerializedProperties.
            foreach (var so in serializedObjects)
            {
                var sp = so.GetIterator();
                while (sp.Next(true))
                    // Replace to appended object.
                    if (sp.propertyType == SerializedPropertyType.ObjectReference &&
                        sp.objectReferenceValue == x.origin)
                        sp.objectReferenceValue = x.newInstance;
            }

            Array.ForEach(serializedObjects, sp => sp.ApplyModifiedProperties());
        }

        private static bool IsAddable(Object assetToAdd, Object mainAsset) =>
            assetToAdd
            && assetToAdd != mainAsset
            && !(assetToAdd is SceneAsset)
            && !(assetToAdd is DefaultAsset)
            && !(assetToAdd is GameObject)
            && !(assetToAdd is Component)
            && !(assetToAdd is Shader)
            && !(assetToAdd is MonoScript)
            && AssetDatabase.Contains(assetToAdd)
            && AssetDatabase.GetAssetPath(assetToAdd) != AssetDatabase.GetAssetPath(mainAsset)
            && !IsBuiltinAsset(assetToAdd);

        private static bool IsBuiltinAsset(Object asset) =>
            AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out var guid, out long _)
            && Regex.IsMatch(guid, "0000000000000000[0-9a-f]000000000000000");

        private class Postprocessor : AssetPostprocessor
        {
            internal static SubAssetEditor s_Editor;

            private static void OnPostprocessAllAssets(
                string[] importedAssets,
                string[] deletedAssets,
                string[] movedAssets,
                string[] movedFromAssetPaths)
            {
                if (!s_Editor || !s_Editor.m_MainAsset || s_Editor.isInGUI) return;

                var path = AssetDatabase.GetAssetPath(s_Editor.m_MainAsset);
                if (importedAssets.Contains(path)
                    || deletedAssets.Contains(path)
                    || movedAssets.Contains(path)
                    || movedFromAssetPaths.Contains(path))
                    s_Editor.Refresh();
            }
        }
    }
}
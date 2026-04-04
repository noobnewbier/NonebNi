using NonebNi.Core.Debug;
using NonebNi.Main;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;
using UnityUtils.GizmosDrawers.Editor;

namespace NonebNi.EditorTools
{
    public class EditorDebugFlagsWindow : EditorWindow
    {
        private IDebugFlagRepository? _debugFlagRepository;
        private NonebGUIDrawer? _drawer;

        private void Update()
        {
            if (EditorApplication.isPlaying)
            {
                if (_debugFlagRepository == null)
                    if (TryActivateWindow())
                        Repaint();
            }
            else
            {
                if (_debugFlagRepository != null)
                {
                    _debugFlagRepository = null;
                    Repaint();
                }
            }
        }

        private void OnEnable()
        {
            var serializedObject = new SerializedObject(this);
            _drawer = new (serializedObject);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);

            if (_drawer == null)
            {
                GUILayout.Label("Initializing...");
                return;
            }

            _drawer.DrawToolbar
            (
                (
                    "Debug Flags",
                    () =>
                    {
                        if (_debugFlagRepository == null)
                        {
                            GUILayout.Label("Debug Flags is only active during play mode and when the active scene is a level");
                            return;
                        }

                        using (_drawer.FlowLayoutScope())
                        {
                            foreach (var (flag, value) in _debugFlagRepository.GetAll())
                            {
                                var newValue = _drawer.DrawToggle(flag, value);
                                if (newValue == value) continue;

                                _debugFlagRepository.Set(flag, newValue);
                            }
                        }
                    }
                ),
                (
                    "Gizmos",
                    () =>
                    {
                        _drawer.DrawGizmosDrawerSettings();
                    }
                )
            );
        }

        [MenuItem("NonebNi/DebugPreference")]
        public static void ShowWindow()
        {
            GetWindow(typeof(EditorDebugFlagsWindow), false, "DebugPreference");
        }

        private bool TryActivateWindow()
        {
            var levelRunner = FindAnyObjectByType<LevelRunner>();
            if (levelRunner == null) return false;

            _debugFlagRepository = levelRunner.DebugTools?.DebugFlagRepository;

            return _debugFlagRepository != null;
        }
    }
}
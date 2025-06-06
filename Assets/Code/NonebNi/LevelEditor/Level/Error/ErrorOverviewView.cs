﻿using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.EditorComponent.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUtils.Editor;
using UnityUtils.Factories;

namespace NonebNi.LevelEditor.Level.Error
{
    /*
     * TODO:
     * this is a most likely, a futile attempt to let future self know where the fuck did he ended up in the past, and how did it happen in the first place
     * I tried to look at your commit history and staging area, and I THINK what happened is that:
     * 1. you were implementing some sort of action system - fair enough
     * 2. somehow, in the middle of implementation, you get very demoralized and stopped working for a while
     * 3. when you are back, you decide to add a pixelated camera and some animation/art to test stuffs, and an attempt to make yourself a bit happy as progress is a bit more visual
     * 4. you got lazy, and rest for quite a long bit again,
     * 5. and then you are back, and started cursing the world why you are not a consistent person at all.
     *
     * speaking, of which, I think you also tried to make some bugs fixing with ErrorOverviewView, I think it's the one where it's not killing itself properly when it should,
     * but I can't recall how, why, if, and what the fuck is it actually fixing, so future me I must apologize.
     */

    public class ErrorOverviewView : IDisposable
    {
        private const float RectWidth = 100;

        private static readonly Vector2 TitleRect = new(RectWidth, 20);
        private static readonly Vector2 ContentRect = new(RectWidth, 100);
        private static readonly Vector2 WindowSize = new(RectWidth, TitleRect.y + ContentRect.y);
        private readonly List<VisualElement> _helperWindowContainer = new();

        private readonly ErrorOverviewPresenter _presenter;
        private readonly Dictionary<SceneView, VisualElement> _sceneViewAndErrorsContainer = new();


        public ErrorOverviewView(IFactory<ErrorOverviewView, ErrorOverviewPresenter> presenterFactory)
        {
            _presenter = presenterFactory.Create(this);
        }

        public void Dispose()
        {
            foreach (var container in _sceneViewAndErrorsContainer.Values)
            {
                container.RemoveFromHierarchy();
            }

            foreach (var container in _helperWindowContainer) container.RemoveFromHierarchy();
        }

        public void OnSceneDraw(Vector2 startingPosition, SceneView sceneView)
        {
            if (!_presenter.IsDrawing)
            {
                if (_sceneViewAndErrorsContainer.TryGetValue(sceneView, out var errorEntryContainer))
                {
                    errorEntryContainer.style.display = DisplayStyle.None;
                }

                return;
            }

            var errors = _presenter.ErrorEntries.ToArray();

            void DrawHelperWindow()
            {
                const string noErrorLabel = "no-error-label";

                if (!_sceneViewAndErrorsContainer.TryGetValue(sceneView, out var errorEntryContainer))
                {
                    const string errorEntryContainerName = "error-entry-container";
                    var inSceneContainer = sceneView.rootVisualElement.Q<ScrollView>(errorEntryContainerName);
                    if (inSceneContainer != null)
                    {
                        //recompile leads to lost of reference - relink it and call it a day
                        errorEntryContainer = inSceneContainer;
                        _sceneViewAndErrorsContainer[sceneView] = errorEntryContainer;
                    }
                    else
                    {
                        var helpWindow = new VisualElement
                        {
                            name = "helper-window-container",
                            style =
                            {
                                position = Position.Absolute,
                                backgroundColor = NonebGUIStyle.SceneHelpBoxColor,

                                width = WindowSize.x,
                                height = WindowSize.y,

                                bottom = -6.5f, //magic value that seems to work well
                                left = startingPosition.x
                            }
                        };
                        _helperWindowContainer.Add(helpWindow);

                        var title = new Label
                        {
                            name = "title-container",
                            text = "Faulty Objects",
                            style =
                            {
                                color = Color.white,
                                unityTextAlign = TextAnchor.MiddleCenter,
                                fontSize = 13,
                                paddingTop = 2,
                                paddingBottom = 2
                            }
                        };

                        errorEntryContainer = new ScrollView
                        {
                            name = errorEntryContainerName
                        };

                        var noErrorHint = new Label("No Error Found")
                        {
                            name = noErrorLabel,
                            style =
                            {
                                color = NonebGUIStyle.HintTextColor
                            }
                        };

                        helpWindow.Add(title);
                        helpWindow.Add(errorEntryContainer);

                        errorEntryContainer.Add(noErrorHint);

                        sceneView.rootVisualElement.Add(helpWindow);

                        _sceneViewAndErrorsContainer[sceneView] = errorEntryContainer;
                    }
                }


                var existingErrorEntries = errorEntryContainer.Query<ErrorEntryView>().ToList();
                var errorSources = errors.GroupBy(e => e.ErrorSource).Select(g => g.Key).ToArray();

                //add new entry if none exist
                foreach (var entity in errorSources.Except(existingErrorEntries.Select(e => e.ErrorSource)))
                    errorEntryContainer.Add(new ErrorEntryView(entity, _presenter));

                //remove existing ones if they are no longer applicable
                foreach (var entry in existingErrorEntries.Where(entry => !errorSources.Contains(entry.ErrorSource)))
                    entry.RemoveFromHierarchy();

                errorEntryContainer.Q<Label>(noErrorLabel).style.display = errors.Any() ?
                    DisplayStyle.None :
                    DisplayStyle.Flex;
                errorEntryContainer.style.display = DisplayStyle.Flex;
            }

            void DrawErrorDescriptionInSceneView()
            {
                var drawnErrors = errors;
                var selectedAnyErrorSource =
                    Selection.gameObjects.Intersect(drawnErrors.Select(e => e.ErrorSource.gameObject)).Any();
                if (selectedAnyErrorSource)
                    drawnErrors = drawnErrors.Where(e => Selection.gameObjects.Contains(e.ErrorSource.gameObject))
                        .ToArray();

                foreach (var errorGroup in drawnErrors.GroupBy(e => e.ErrorSource))
                {
                    var sourceWorldPos = errorGroup.Key.transform.position;
                    var labelBottomLeftWorldPos = sourceWorldPos + Vector3.up * 0.25f;
                    var labelContent = EditorGUIUtility.TrTempContent(
                        errorGroup.Select((e, i) => $"{i + 1}. {e.Description}")
                            .Aggregate((current, next) => $"{current}\n\n{next}")
                    );
                    var labelSizedRect = HandleUtility.WorldPointToSizedRect(
                        labelBottomLeftWorldPos,
                        labelContent,
                        NonebGUIStyle.SceneErrorBox
                    );

                    using (new HandlesGUIScope())
                    {
                        if (!Selection.gameObjects.Contains(errorGroup.Key.gameObject))
                        {
                            var arrowTargetPos = labelSizedRect.center -
                                                 Vector2.up * labelSizedRect.height / 2f +
                                                 Vector2.left * labelSizedRect.width / 4f;
                            var sourceScreenPos = HandleUtility.WorldToGUIPoint(sourceWorldPos);
                            Handles.DrawDottedLine(sourceScreenPos, arrowTargetPos, 3);
                        }

                        var labelBottomScreenPos = HandleUtility.WorldToGUIPoint(labelBottomLeftWorldPos);
                        GUI.Label(
                            new Rect(
                                labelBottomScreenPos.x,
                                labelBottomScreenPos.y - labelSizedRect.height,
                                labelSizedRect.width,
                                labelSizedRect.height
                            ),
                            labelContent,
                            NonebGUIStyle.SceneErrorBox
                        );
                    }
                }
            }

            DrawHelperWindow();
            DrawErrorDescriptionInSceneView();
        }

        private class HandlesGUIScope : IDisposable
        {
            private bool _disposed;

            public HandlesGUIScope()
            {
                Handles.BeginGUI();
            }

            public void Dispose()
            {
                if (_disposed) return;

                Handles.EndGUI();
            }
        }

        private class ErrorEntryView : VisualElement
        {
            public readonly EditorEntity ErrorSource;

            public ErrorEntryView(EditorEntity errorSource, ErrorOverviewPresenter presenter)
            {
                ErrorSource = errorSource;

                style.flexDirection = FlexDirection.Row;

                var jumpToButton = new Button(() => presenter.OnClickErrorNavigationButton(errorSource))
                {
                    text = "?",
                    style = { flexGrow = 0 }
                };

                var label = new Label(errorSource.name)
                {
                    style =
                    {
                        color = NonebGUIStyle.ErrorTextColor,
                        unityTextAlign = TextAnchor.MiddleLeft
                    }
                };

                Add(jumpToButton);
                Add(label);
            }
        }
    }
}
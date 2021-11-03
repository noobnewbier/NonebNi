using System.Linq;
using NonebNi.Editors.Di;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.Editors.Level.Error
{
    public class ErrorOverviewView
    {
        private const float RectWidth = 100;

        private static readonly Vector2 TitleRect = new Vector2(RectWidth, 20);
        private static readonly Vector2 ContentRect = new Vector2(RectWidth, 100);
        public static readonly Vector2 WindowSize = new Vector2(RectWidth, TitleRect.y + ContentRect.y);

        private readonly ErrorOverviewPresenter _presenter;

        private Vector2 _currentScrollPosition = Vector2.zero;

        public ErrorOverviewView(ILevelEditorComponent component)
        {
            _presenter = component.CreateErrorOverviewPresenter(this);
        }

        public void OnSceneDraw(Vector2 startingPosition)
        {
            if (!_presenter.IsDrawing) return;

            var errors = _presenter.ErrorEntries.ToArray();

            void DrawHelperWindow()
            {
                Handles.BeginGUI();

                var fullRect = new Rect(startingPosition, WindowSize);
                GUI.Box(fullRect, GUIContent.none, NonebGUIStyle.SceneHelpWindow);

                var titleRect = new Rect(startingPosition, TitleRect);
                GUI.Label(titleRect, "ErrorOverview", NonebGUIStyle.Title);

                var scrollRect = new Rect(startingPosition + Vector2.up * TitleRect.y, ContentRect);
                using (new GUILayout.AreaScope(scrollRect))
                {
                    using (var scrollView = new EditorGUILayout.ScrollViewScope(_currentScrollPosition))
                    {
                        _currentScrollPosition = scrollView.scrollPosition;
                        foreach (var entry in errors)
                            using (new GUILayout.HorizontalScope())
                            {
                                if (GUILayout.Button("?", GUILayout.ExpandWidth(false)))
                                    _presenter.OnClickErrorNavigationButton(entry);

                                GUILayout.Label($"{entry.ErrorSource.name}", NonebGUIStyle.Error);
                            }

                        if (!errors.Any()) GUILayout.Label("No Error Found", NonebGUIStyle.Hint);
                    }
                }

                Handles.EndGUI();
            }

            void DrawErrorDescriptionInSceneView()
            {
                var drawnErrors = errors;
                var selectedAnyErrorSource =
                    Selection.gameObjects.Intersect(drawnErrors.Select(e => e.ErrorSource.gameObject)).Any();
                if (selectedAnyErrorSource)
                    drawnErrors = drawnErrors.Where(e => Selection.gameObjects.Contains(e.ErrorSource.gameObject)).ToArray();

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

                    Handles.BeginGUI();

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
                    Handles.EndGUI();
                }
            }


            DrawHelperWindow();
            DrawErrorDescriptionInSceneView();
        }
    }
}
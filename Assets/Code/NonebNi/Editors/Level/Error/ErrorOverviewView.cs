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
            Handles.BeginGUI();

            var fullRect = new Rect(startingPosition, WindowSize);
            GUI.Box(fullRect, GUIContent.none, NonebGUIStyle.SceneHelpBox);

            var titleRect = new Rect(startingPosition, TitleRect);
            GUI.Label(titleRect, "ErrorOverview", NonebGUIStyle.Title);

            var scrollRect = new Rect(startingPosition + Vector2.up * TitleRect.y, ContentRect);
            using (new GUILayout.AreaScope(scrollRect))
            {
                _currentScrollPosition = EditorGUILayout.BeginScrollView(_currentScrollPosition);
                var errors = _presenter.ErrorEntries.ToArray();
                foreach (var entry in errors)
                {
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("?", GUILayout.ExpandWidth(false)))
                    {
                        _presenter.OnClickErrorNavigationButton(entry);
                    }

                    GUILayout.Label($"{entry.ErrorSource.name}", NonebGUIStyle.Error);

                    GUILayout.EndHorizontal();
                }

                if (!errors.Any()) GUILayout.Label("No Error Found", NonebGUIStyle.Hint);

                EditorGUILayout.EndScrollView();
            }

            Handles.EndGUI();
        }
    }
}
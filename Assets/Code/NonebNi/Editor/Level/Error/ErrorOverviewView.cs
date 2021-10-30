using NonebNi.Editor.Di;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Editor.Level.Error
{
    public class ErrorOverviewView
    {
        private const float RectHeight = 100;
        private const float RectWidth = 100;
        public static readonly Vector2 WindowSize = new Vector2(RectWidth, RectHeight);

        private static readonly int WindowID = nameof(ErrorOverviewView).GetHashCode();

        private readonly ErrorOverviewPresenter _presenter;

        private Vector2 _currentScrollPosition = Vector2.zero;

        public ErrorOverviewView(ILevelEditorComponent component)
        {
            _presenter = component.CreateErrorOverviewPresenter(this);
        }

        public void OnSceneDraw(Vector2 position)
        {
            if (!_presenter.IsDrawing) return;

            var rect = new Rect(position, WindowSize);

            void WindowFunc(int _)
            {
                _currentScrollPosition = GUILayout.BeginScrollView(_currentScrollPosition);

                var errors = _presenter.ErrorEntries;
                foreach (var entry in errors)
                {
                    GUILayout.BeginHorizontal();

                    using (new EditorGUI.DisabledScope())
                    {
                        EditorGUILayout.ObjectField(entry.ErrorSource, typeof(Object), false);
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndScrollView();
            }

            GUI.Window(
                WindowID,
                rect,
                WindowFunc,
                "ErrorOverview"
            );
        }
    }
}
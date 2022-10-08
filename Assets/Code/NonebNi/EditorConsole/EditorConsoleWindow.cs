using NonebNi.Main;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.EditorConsole
{
    public class EditorConsoleWindow : EditorWindow
    {
        private NonebDebugConsole? _console;
        private Vector2 _consoleScrollPos;

        private string _currentPlayerInput = string.Empty;

        private void Update()
        {
            if (EditorApplication.isPlaying)
            {
                if (_console == null)
                    if (TryActivateConsole())
                        Repaint();
            }
            else
            {
                if (_console != null)
                {
                    _console = null;
                    Repaint();
                }
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(10);

            if (_console == null)
            {
                GUILayout.Label("Console is only active during play mode and when the active scene is a level");
                return;
            }

            using (new GUILayout.HorizontalScope())
            {
                DrawUserTextInput();
                if (GUILayout.Button("Enter", GUILayout.ExpandWidth(false))) _console.InterpretInput(_currentPlayerInput);
            }

            NonebEditorGUI.ShowHorizontalLine();

            DrawConsoleOutput();

            void DrawUserTextInput()
            {
                const string userTextInputControl = nameof(userTextInputControl);

                GUI.SetNextControlName(userTextInputControl);
                _currentPlayerInput = EditorGUILayout.TextField("Command: ", _currentPlayerInput);

                if (Event.current.isKey &&
                    Event.current.keyCode == KeyCode.Return &&
                    Event.current.type == EventType.KeyUp &&
                    GUI.GetNameOfFocusedControl() == userTextInputControl)
                {
                    Event.current.Use();

                    EditorGUI.FocusTextInControl(userTextInputControl);

                    _console.InterpretInput(_currentPlayerInput);
                }
            }

            void DrawConsoleOutput()
            {
                using (var scrollView = new GUILayout.ScrollViewScope(_consoleScrollPos))
                {
                    _consoleScrollPos = scrollView.scrollPosition;

                    GUILayout.Label(_console.AccumulatedOutput.ToString());
                }
            }
        }

        [MenuItem("NonebNi/EditorConsole")]
        public static void ShowWindow()
        {
            GetWindow(typeof(EditorConsoleWindow), false, "EditorConsole");
        }

        private bool TryActivateConsole()
        {
            var levelComponent = FindObjectOfType<Level>()?.LevelComponent;
            if (levelComponent == null) return false;

            var commandsDataRepository = new CommandsDataRepository();
            var commandHandler = new CommandHandler(
                levelComponent.GetCommandEvaluationService(),
                levelComponent.GetLevelData().Map,
                levelComponent.GetSequencePlayer(),
                commandsDataRepository
            );
            var parser = new ExpressionParser(commandsDataRepository);
            var lexer = new TextLexer();
            _console = new NonebDebugConsole(commandHandler, parser, lexer);

            return true;
        }
    }
}
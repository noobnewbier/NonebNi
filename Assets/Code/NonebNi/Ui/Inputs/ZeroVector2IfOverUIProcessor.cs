using Noneb.UI.InputSystems;
using NonebNi.Core.GameContexts;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NonebNi.Ui.Inputs
{
    /// <summary>
    /// I don't know why it needs the if unity_editor thing but I stopped asking question, follow the doc dude.
    /// https://docs.unity3d.com/Packages/com.unity.inputsystem@1.14/manual/ProcessorTypes.html
    /// </summary>

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public class ZeroVector2IfOverUIProcessor : InputProcessor<Vector2>
    {
#if UNITY_EDITOR
        static ZeroVector2IfOverUIProcessor()
        {
            Initialize();
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            InputSystem.RegisterProcessor<ZeroVector2IfOverUIProcessor>();
        }

        public override Vector2 Process(Vector2 value, InputControl control)
        {
            var deps = GameContext.FindImmediate<Dependencies>();
            if (deps?.InputSystem == null) return value;

            if (deps.InputSystem.IsMouseOverUi) return Vector2.zero;

            return value;
        }

        public record Dependencies(IInputSystem InputSystem);
    }
}
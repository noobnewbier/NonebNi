using Noneb.UI.InputSystems.Event;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Noneb.UI.InputSystems
{
    public class NonebInputSystem : IInputSystem
    {
        public Vector3 MousePosition => Input.mousePosition;
        public bool LeftClick => Input.GetMouseButton(0);
        public event NEventHandler<AnyInput> AnyInput;
    }
}
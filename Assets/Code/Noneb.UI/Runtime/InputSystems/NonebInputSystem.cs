using Noneb.UI.InputSystems.Event;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using Vector3 = UnityEngine.Vector3;

namespace Noneb.UI.InputSystems
{
    public class NonebInputSystem : IInputSystem
    {
        public Vector3 MousePosition => Input.mousePosition;
        public bool LeftClick => Input.GetMouseButton(0);
        public event NEventHandler<AnyInput> AnyInput;

        public bool IsMouseOverUi
        {
            get
            {
                // [Works with PhysicsRaycaster on the Camera. Requires New Input System. Assumes mouse.)
                if (EventSystem.current == null) return false;

                if (EventSystem.current.currentInputModule is not InputSystemUIInputModule uiInputModule) return false;

                var lastRaycastResult = uiInputModule.GetLastRaycastResult(Mouse.current.deviceId);

                var layer = LayerMask.NameToLayer("UI");
                return lastRaycastResult.gameObject != null && lastRaycastResult.gameObject.layer == layer;
            }
        }
    }
}
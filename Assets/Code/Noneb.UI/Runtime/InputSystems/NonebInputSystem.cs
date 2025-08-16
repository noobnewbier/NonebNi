using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Noneb.UI.InputSystems
{
    public class NonebInputSystem : IInputSystem
    {
        private readonly InputActionAsset _asset;

        public NonebInputSystem(InputActionAsset asset)
        {
            _asset = asset;
        }

        public InputAction FindAction(string name)
        {
            var action = _asset.FindAction(name);

            return action;
        }

        public bool GetAction(string name)
        {
            var action = FindAction(name);

            return action.WasPerformedThisFrame();
        }

        public T ReadValue<T>(string name) where T : struct
        {
            var action = FindAction(name);

            return action.ReadValue<T>();
        }

        public void SetActionMapActive(string name, bool active)
        {
            var map = _asset.FindActionMap(name);

            if (active)
                map.Enable();
            else
                map.Disable();
        }

        public bool IsMouseOverUi
        {
            get
            {
                //Note: can we bake it in action processor?

                // Works with PhysicsRaycaster on the Camera. Requires New Input System. Assumes mouse.
                if (EventSystem.current == null) return false;

                if (EventSystem.current.currentInputModule is not InputSystemUIInputModule uiInputModule) return false;

                var lastRaycastResult = uiInputModule.GetLastRaycastResult(Mouse.current.deviceId);

                var layer = LayerMask.NameToLayer("UI");
                return lastRaycastResult.gameObject != null && lastRaycastResult.gameObject.layer == layer;
            }
        }
    }
}
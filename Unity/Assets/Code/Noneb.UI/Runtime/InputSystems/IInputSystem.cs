using UnityEngine.InputSystem;

namespace Noneb.UI.InputSystems
{
    public interface IInputSystem
    {
        bool IsMouseOverUi { get; }
        InputAction FindAction(string name);
        bool GetAction(string name);
        void SetActionMapActive(string name, bool active);
        T ReadValue<T>(string name) where T : struct;
    }
}
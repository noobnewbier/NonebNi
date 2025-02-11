using Noneb.UI.InputSystems.Event;
using Vector3 = UnityEngine.Vector3;

namespace Noneb.UI.InputSystems
{
    public interface IInputSystem
    {
        Vector3 MousePosition { get; }

        bool LeftClick { get; }

        //TODO: can't remember the me 4 years ago why am I having this?
        event NEventHandler<AnyInput> AnyInput;
    }
}
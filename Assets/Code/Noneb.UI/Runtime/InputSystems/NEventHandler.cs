using Noneb.UI.InputSystems.Event;

namespace Noneb.UI.InputSystems
{
    public delegate void NEventHandler<in TArg>(TArg arg) where TArg : InputEvent;
}
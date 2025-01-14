namespace Noneb.UI.InputSystems.Event
{
    /// <summary>
    /// I will be honest, I am not entirely sure why I have this. I think it's to respond to console input?
    /// It looks like "something Q-Games would have done" plus "younger peter trying to add his own idea"...
    /// Anyway, future me this is something you need to figure out.
    /// </summary>
    public class AnyInput : InputEvent
    {
        public static AnyInput Instance { get; } = new();
    }
}
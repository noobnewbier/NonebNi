namespace NonebNi.Ui.UIContexts
{
    public interface ISharedInContext { }

    /// <summary>
    /// This is weird, but I can't come up with a way to do this locator shenanigans easily
    /// </summary>
    public class SharedContextInitializer
    {
        private readonly ISharedInContext[] _totInit;

        public SharedContextInitializer(ISharedInContext[] totInit)
        {
            _totInit = totInit;
        }

        public void Init()
        {
            foreach (var context in _totInit) UIContext.Set(context.GetType(), context);
        }
    }
}
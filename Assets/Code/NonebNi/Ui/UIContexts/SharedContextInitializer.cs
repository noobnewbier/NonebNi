namespace NonebNi.Ui.UIContexts
{
    /// <summary>
    /// This is weird, but I can't come up with a way to do this locator shenanigans easily
    /// </summary>
    public class SharedContextInitializer
    {
        private readonly object[] _totInit;

        public SharedContextInitializer(object[] totInit)
        {
            _totInit = totInit;
        }

        public void Init()
        {
            foreach (var context in _totInit) UIContext.Set(context.GetType(), context);
        }
    }
}
namespace NonebNi.Core.GameContexts
{
    /// <summary>
    /// Hacky way to allow us to do something akin to named injection
    /// </summary>
    public abstract record DiKeys
    {
        public record PlayerAgent : DiKeys;
    }

    public record KeyedInject<TKey, TValue>(TValue Value)
    {
        public static implicit operator TValue(KeyedInject<TKey, TValue> @this) => @this.Value;
    }
}
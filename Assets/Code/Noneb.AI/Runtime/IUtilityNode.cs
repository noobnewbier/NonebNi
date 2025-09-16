namespace Noneb.AI.Runtime
{
    public interface IUtilityNode
    {
        /// <summary>
        /// There's no restriction that this is normalized,
        /// it's the designer/coder(both me) responsibility to make sure the graph is using the normalized value,
        /// one way or another
        /// </summary>
        public Utility FindUtility();
    }
}
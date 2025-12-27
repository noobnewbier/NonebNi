using System.Collections.Generic;
using NonebNi.Core.Commands;
using NonebNi.Core.Units;

namespace NonebNi.Core.AI
{
    /// <summary>
    /// Note
    /// - If possible I really don't want the "core" to be based on Unity's package, as it inevitably introduces dependencies
    /// to Unity which I originally intend to keep under NonebNi.UI..
    /// </summary>
    public interface IUtilityNode
    {
        /// <summary>
        /// There's no restriction that this is normalized,
        /// it's the designer/coder(both me) responsibility to make sure the graph is using the normalized value,
        /// one way or another
        /// Note:
        /// Maybe it's better to have the command passes down and here we only evaluate,
        /// stuffs that doesn't get evaluated is naturally the bottom of the list - that way we only need to find the options once
        /// Perhaps is something that we do for optimization but for now it's good enough.
        /// In fact - that's a bad idea, if we need caching better do it in the option finder and have it dirty itself on map state
        /// changing.
        /// </summary>
        public IAsyncEnumerable<(ActionCommand command, Utility utility)> FindActionAndUtility(UnitData controlledUnit);
    }
}
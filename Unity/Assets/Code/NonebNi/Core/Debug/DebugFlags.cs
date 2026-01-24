using System;
using System.Collections.Generic;

namespace NonebNi.Core.Debug
{
    [Serializable]
    public class DebugFlags
    {
        /*
         * Note:
         * if obsoleted keys become a real headache, we can always implement a time based obsolete mechanism,
         * where if keys aren't accessed for X amount of time it's hidden from the debug view and eventually destroyed?
         *
         * Anyway, I can regret later.
         */
        public Dictionary<string, bool> Flags = new();
    }
}
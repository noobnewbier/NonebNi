using System.Collections.Generic;

namespace Noneb.Tags.Runtime
{
    internal interface INonebTagSource
    {
        public string Name { get; }

        public IEnumerable<NonebTag> GetTags();
        string FindDescription(NonebTag tag);
    }
}
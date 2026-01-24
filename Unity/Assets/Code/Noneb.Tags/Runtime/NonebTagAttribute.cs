using System;

namespace Noneb.Tags.Runtime
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = true)]
    public class NonebTagAttribute : Attribute
    {
        public NonebTagAttribute(string description = "")
        {
            Description = description;
        }

        public string Description { get; }
    }
}
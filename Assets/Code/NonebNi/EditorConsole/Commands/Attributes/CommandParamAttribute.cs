using System;

namespace NonebNi.EditorConsole.Commands.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class CommandParamAttribute : Attribute
    {
        public readonly string Description;

        public CommandParamAttribute(string description)
        {
            Description = description;
        }
    }
}
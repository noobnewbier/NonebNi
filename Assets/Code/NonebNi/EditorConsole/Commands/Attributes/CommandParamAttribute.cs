using System;

namespace NonebNi.EditorConsole.Commands.Attributes
{
    /// <summary>
    ///     Description of any command parameter, expected to be used in parameter for commands' constructors
    /// </summary>
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
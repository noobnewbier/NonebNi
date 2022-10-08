using System;

namespace NonebNi.EditorConsole.Commands.Attributes
{
    /// <summary>
    /// Tagging a command using <see cref="CommandAttribute"/> makes it available for users in the debug console.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CommandAttribute : Attribute
    {
        public CommandAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// How it is referred by the user, case sensitive. Must not collide with other existing commands
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Description of the command, can be left empty
        /// </summary>
        public string Description { get; }
    }
}
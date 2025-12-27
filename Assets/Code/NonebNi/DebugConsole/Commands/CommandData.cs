using System;

namespace NonebNi.DebugConsole.Commands
{
    public class CommandData
    {
        public CommandData(Type commandType, string name, string description)
        {
            Name = name;
            Description = description;
            CommandType = commandType;
        }

        public Type CommandType { get; }
        public string Name { get; }
        public string Description { get; }
    }
}
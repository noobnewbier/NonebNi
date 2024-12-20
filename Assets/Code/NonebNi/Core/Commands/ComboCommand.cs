using System.Collections.Generic;

namespace NonebNi.Core.Commands
{
    public class ComboCommand : ICommand
    {
        public readonly IReadOnlyList<ICommand> Commands;

        public ComboCommand(params ICommand[] commands)
        {
            Commands = commands;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;
using UnityUtils;

namespace NonebNi.EditorConsole.Commands
{
    public interface ICommandsDataRepository
    {
        bool TryGetCommand(string? commandName, [NotNullWhen(true)] out CommandData? data);
        IEnumerable<CommandData> GetAllCommands();
    }

    public class CommandsDataRepository : ICommandsDataRepository
    {
        private readonly Dictionary<string, CommandData> _manual;

        public CommandsDataRepository()
        {
            var typesWithAttribute = ReflectionUtils.GetTypesWithAttribute<CommandAttribute>().ToArray();
            var typeWithAttributeAndImplIConsoleCommand =
                typesWithAttribute.Where(v => typeof(IConsoleCommand).IsAssignableFrom(v.type)).ToArray();

            foreach (var erroneousType in typesWithAttribute.Except(typeWithAttributeAndImplIConsoleCommand))
                Debug.LogError(
                    $"{erroneousType.type.Name} is marked as a command but does not implement {nameof(IConsoleCommand)} - ignored in console app");

            foreach (var duplicatedNameGroup in typeWithAttributeAndImplIConsoleCommand
                         .GroupBy(v => v.attachedAttribute.Name)
                         .Where(g => g.Count() > 1))
                throw new InvalidOperationException(
                    $"Commands \"{string.Join(",", duplicatedNameGroup.Select(v => v.type.FullName))}\" are sharing the same alias - unable to proceed as there's no way to identify which command is being called when they share the same alias");

            _manual = typeWithAttributeAndImplIConsoleCommand
                .Select(v => new CommandData(v.type, v.attachedAttribute.Name, v.attachedAttribute.Description))
                .ToDictionary(d => d.Name, d => d);
        }

        public bool TryGetCommand(string? commandName, [NotNullWhen(true)] out CommandData? data)
        {
            if (commandName != null) return _manual.TryGetValue(commandName, out data);

            data = null;
            return false;
        }

        public IEnumerable<CommandData> GetAllCommands()
        {
            return _manual.Values;
        }
    }
}
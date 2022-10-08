using System.Linq;
using System.Text;
using NonebNi.Core.Commands;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using NonebNi.Core.Units;
using NonebNi.EditorConsole.Commands;
using UnityUtils;

namespace NonebNi.EditorConsole
{
    public class CommandHandler
    {
        private readonly ICommandEvaluationService _commandEvaluationService;
        private readonly ICommandsDataRepository _commandsDataRepository;
        private readonly IReadOnlyMap _readOnlyMap;
        private readonly ISequencePlayer _sequencePlayer;

        public CommandHandler(ICommandEvaluationService commandEvaluationService,
            IReadOnlyMap readOnlyMap,
            ISequencePlayer sequencePlayer,
            ICommandsDataRepository commandsDataRepository)
        {
            _commandEvaluationService = commandEvaluationService;
            _readOnlyMap = readOnlyMap;
            _sequencePlayer = sequencePlayer;
            _commandsDataRepository = commandsDataRepository;
        }

        public void Handle(IConsoleCommand command, StringBuilder outputBuffer)
        {
            switch (command)
            {
                case DamageConsoleCommand damageConsoleCommand:
                    var isValid = _readOnlyMap.TryGet<UnitData>(damageConsoleCommand.Coordinate, out var unitData);
                    if (isValid)
                    {
                        var damageCommand = new DamageCommand(damageConsoleCommand.Damage, unitData);
                        var sequences = _commandEvaluationService.Evaluate(damageCommand);

                        foreach (var sequence in sequences) _sequencePlayer.Play(sequence);
                    }

                    break;
                case ErrorMessageConsoleCommand errorMessageConsoleCommand:
                    outputBuffer.Append(errorMessageConsoleCommand.Message);
                    outputBuffer.AppendLine();
                    break;
                case ClearConsoleCommand _:
                    outputBuffer.Clear();
                    break;

                case HelpCommand helpCommand:
                    if (string.IsNullOrWhiteSpace(helpCommand.CommandName))
                        PrintHelpMessageForAllCommands();
                    else if (_commandsDataRepository.TryGetCommand(helpCommand.CommandName, out var d))
                        PrintHelpMessageForCommand(d);

                    outputBuffer.AppendLine();
                    break;

                    void PrintHelpMessageForAllCommands()
                    {
                        foreach (var commandData in _commandsDataRepository.GetAllCommands())
                        {
                            outputBuffer.Append(commandData.Name);
                            outputBuffer.AppendLine();
                            outputBuffer.Append(
                                $@"---
Command description:
    {commandData.Description}
");
                            outputBuffer.AppendLine();
                        }
                    }

                    void PrintHelpMessageForCommand(CommandData commandData)
                    {
                        outputBuffer.Append(commandData.Name);
                        outputBuffer.AppendLine();
                        outputBuffer.Append(
                            $@"---
Command description:
    {commandData.Description}
");
                        foreach (var constructorInfo in commandData.CommandType.GetConstructors())
                        {
                            var parameters = constructorInfo.GetParameters();
                            var commandFormat = parameters.Any()
                                ? string.Join(", ", parameters.Select(p => p.Name))
                                : "NO_ARG";

                            outputBuffer.AppendLine($"Signature: [{commandFormat}]");

                            var signatureInfos = constructorInfo.GetAttribute<SignatureInfoAttribute>(false);
                            var signatureDescription = signatureInfos?.Description;
                            outputBuffer.AppendLine($"- Description: {signatureDescription}");

                            foreach (var parameter in parameters)
                            {
                                var commandParams =
                                    parameter.GetAttributes<CommandParamAttribute>(false);
                                foreach (var paramInfo in commandParams)
                                    outputBuffer.AppendLine($"   - {parameter.Name}: {paramInfo.Description}");
                            }

                            outputBuffer.AppendLine();
                        }
                    }
            }
        }
    }
}
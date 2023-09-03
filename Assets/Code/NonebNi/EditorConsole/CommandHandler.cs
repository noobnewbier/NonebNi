using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Actions;
using NonebNi.Core.Agents;
using NonebNi.Core.Commands;
using NonebNi.Core.Decisions;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using NonebNi.Core.Units;
using NonebNi.EditorConsole.Commands;
using NonebNi.EditorConsole.Commands.Attributes;
using UnityUtils;

namespace NonebNi.EditorConsole
{
    public class CommandHandler
    {
        private readonly IActionRepository _actionRepository;
        private readonly IAgentsService _agentsService;
        private readonly ICommandEvaluationService _commandEvaluationService;
        private readonly ICommandsDataRepository _commandsDataRepository;
        private readonly IReadOnlyMap _readOnlyMap;
        private readonly ISequencePlayer _sequencePlayer;
        private readonly IUnitTurnOrderer _turnOrderer;

        public CommandHandler(
            ICommandEvaluationService commandEvaluationService,
            IReadOnlyMap readOnlyMap,
            ISequencePlayer sequencePlayer,
            ICommandsDataRepository commandsDataRepository,
            IAgentsService agentsService,
            IUnitTurnOrderer turnOrderer,
            IActionRepository actionRepository)
        {
            _commandEvaluationService = commandEvaluationService;
            _readOnlyMap = readOnlyMap;
            _sequencePlayer = sequencePlayer;
            _commandsDataRepository = commandsDataRepository;
            _agentsService = agentsService;
            _turnOrderer = turnOrderer;
            _actionRepository = actionRepository;
        }

        public void Handle(IConsoleCommand command, StringBuilder outputBuffer)
        {
            switch (command)
            {
                #region Command Evaluation

                case TeleportConsoleCommand teleportCommand:
                {
                    if (_readOnlyMap.TryGet<UnitData>(teleportCommand.StartPos, out var unit))
                        _ = EvaluateSequence(new TeleportCommand(unit, teleportCommand.TargetPos));
                    break;
                }

                case DamageConsoleCommand damageConsoleCommand:
                {
                    if (_readOnlyMap.TryGet<UnitData>(damageConsoleCommand.Coordinate, out var unit))
                        _ = EvaluateSequence(new DamageCommand(damageConsoleCommand.Damage, unit));
                    break;
                }

                case MoveConsoleCommand moveConsoleCommand:
                    _agentsService.OverrideDecision(
                        new ActionDecision(
                            ActionDatas.MoveAction,
                            _turnOrderer.CurrentUnit,
                            moveConsoleCommand.TargetPos
                        )
                    );
                    break;

                case ActionConsoleCommand actionConsoleCommand:
                {
                    var action = _actionRepository.GetAction(actionConsoleCommand.ActionId);
                    if (action == null)
                    {
                        outputBuffer.AppendLine(
                            $"Unable to find action with matching action ID: {actionConsoleCommand.ActionId}"
                        );
                        break;
                    }

                    UnitData? unit;
                    if (!actionConsoleCommand.ActorCoord.HasValue)
                    {
                        unit = _turnOrderer.CurrentUnit;
                    }
                    else if (!_readOnlyMap.TryGet(actionConsoleCommand.ActorCoord.Value, out unit))
                    {
                        break;
                    }

                    _ = EvaluateSequence(new ActionCommand(action, unit, actionConsoleCommand.TargetCoord));

                    break;
                }

                #endregion

                case EndTurnDecisionCommand:
                    _agentsService.OverrideDecision(EndTurnDecision.Instance);
                    break;

                case ErrorMessageConsoleCommand errorMessageConsoleCommand:
                    outputBuffer.Append(errorMessageConsoleCommand.Message);
                    outputBuffer.AppendLine();
                    break;

                case ClearConsoleCommand:
                    outputBuffer.Clear();
                    break;

                case HelpCommand helpCommand:
                    if (string.IsNullOrWhiteSpace(helpCommand.CommandName))
                        PrintHelpMessageForAllCommands();
                    else if (_commandsDataRepository.TryGetCommand(helpCommand.CommandName, out var d))
                        PrintHelpMessageForCommand(d);

                    outputBuffer.AppendLine();

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
"
                            );
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
"
                        );
                        foreach (var constructorInfo in commandData.CommandType.GetConstructors())
                        {
                            var parameters = constructorInfo.GetParameters();
                            var commandFormat = parameters.Any() ?
                                string.Join(", ", parameters.Select(p => p.Name)) :
                                "NO_ARG";

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

                    break;
            }
        }

        private async UniTask EvaluateSequence(ICommand command)
        {
            var sequences = _commandEvaluationService.Evaluate(command);

            await _sequencePlayer.Play(sequences).ToUniTask();
        }
    }
}
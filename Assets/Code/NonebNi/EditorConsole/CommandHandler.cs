using System.Text;
using NonebNi.Core.Commands;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using NonebNi.Core.Units;
using NonebNi.EditorConsole.Commands;

namespace NonebNi.EditorConsole
{
    public class CommandHandler
    {
        private readonly ICommandEvaluationService _commandEvaluationService;
        private readonly IReadOnlyMap _readOnlyMap;
        private readonly ISequencePlayer _sequencePlayer;

        public CommandHandler(ICommandEvaluationService commandEvaluationService,
                              IReadOnlyMap readOnlyMap,
                              ISequencePlayer sequencePlayer)
        {
            _commandEvaluationService = commandEvaluationService;
            _readOnlyMap = readOnlyMap;
            _sequencePlayer = sequencePlayer;
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
            }
        }
    }
}
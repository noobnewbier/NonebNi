using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Actions;
using NonebNi.Core.Agents;
using NonebNi.Core.Commands;
using NonebNi.Core.Decisions;
using NonebNi.Core.Entities;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Maps;
using NonebNi.Core.Units;
using NonebNi.DebugConsole.Commands;
using NonebNi.DebugConsole.Commands.Attributes;
using UnityUtils;

namespace NonebNi.DebugConsole
{
    public class CommandHandler
    {
        private readonly IActionCommandEvaluator _actionCommandEvaluator;
        private readonly IActionRepository _actionRepository;
        private readonly IAgentsService _agentsService;
        private readonly ICommandsDataRepository _commandsDataRepository;
        private readonly ILevelFlowController _levelFlowController;
        private readonly IReadOnlyMap _readOnlyMap;
        private readonly IUnitTurnOrderer _turnOrderer;

        public CommandHandler(
            IActionCommandEvaluator actionCommandEvaluator,
            IReadOnlyMap readOnlyMap,
            ICommandsDataRepository commandsDataRepository,
            IAgentsService agentsService,
            IUnitTurnOrderer turnOrderer,
            IActionRepository actionRepository,
            ILevelFlowController levelFlowController)
        {
            _actionCommandEvaluator = actionCommandEvaluator;
            _readOnlyMap = readOnlyMap;
            _commandsDataRepository = commandsDataRepository;
            _agentsService = agentsService;
            _turnOrderer = turnOrderer;
            _actionRepository = actionRepository;
            _levelFlowController = levelFlowController;
        }

        public async UniTask Handle(IConsoleCommand command, StringBuilder outputBuffer)
        {
            await DoHandle(command as dynamic, outputBuffer);
        }

        private UniTask DoHandle(IConsoleCommand command, StringBuilder outputBuffer)
        {
            var errorCommand = new ErrorMessageConsoleCommand($"Command handler is missing implementation for type {command.GetType()}. Noop");
            return DoHandle(errorCommand, outputBuffer);
        }

        private void EvaluateSequence(ActionCommand command)
        {
            var result = _actionCommandEvaluator.Evaluate(command);
            var @event = new LevelEvent.SequenceOccured(result);

            _levelFlowController.ForcePlayEvent(@event);
        }
    }
}
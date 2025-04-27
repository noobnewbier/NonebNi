using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Agents;
using NonebNi.Core.Combos;
using NonebNi.Core.Commands;
using NonebNi.Core.Decisions;
using NonebNi.Core.Units;
using Unity.Logging;

namespace NonebNi.Core.FlowControl
{
    public interface ILevelFlowController
    {
        IAgentsService AgentsService { get; }
        IActionCommandEvaluator Evaluator { get; }
        IUnitTurnOrderer UnitTurnOrderer { get; }
        ChannelReader<LevelEvent> Run();
        void ForcePlayEvent(LevelEvent levelEvent);
    }

    public class LevelFlowController : ILevelFlowController
    {
        public LevelFlowController(
            IActionCommandEvaluator evaluator,
            IUnitTurnOrderer unitTurnOrderer,
            IAgentsService agentService,
            IDecisionValidator decisionValidator,
            IComboChecker comboChecker)
        {
            Evaluator = evaluator;
            UnitTurnOrderer = unitTurnOrderer;
            AgentsService = agentService;
            DecisionValidator = decisionValidator;
            _comboChecker = comboChecker;

            _eventChannel = Channel.CreateSingleConsumerUnbounded<LevelEvent>();
        }

        public IDecisionValidator DecisionValidator { get; }
        public IAgentsService AgentsService { get; }
        public IActionCommandEvaluator Evaluator { get; }
        public IUnitTurnOrderer UnitTurnOrderer { get; }

        private readonly IComboChecker _comboChecker;

        /// <summary>
        /// If we need a state machine, we can add one later, atm a simple event queue would suffice
        /// </summary>
        /// <returns></returns>
        private readonly Channel<LevelEvent> _eventChannel;

        public ChannelReader<LevelEvent> Run()
        {
            //todo: at some point we need a way to kill it.
            RunLevelFlow().Forget();

            return _eventChannel.Reader;
        }

        private async UniTask RunLevelFlow()
        {
            //TODO: replace all these logging w/ a Decorator using StrongInject.

            var turnNum = 0; //Mostly for debug purposes - but probably necessary for UI at some point
            while (true)
            {
                var currentUnit = UnitTurnOrderer.CurrentUnit;
                Log.Info($"[Level] Turn {turnNum}, {currentUnit.Name}'s turn");

                currentUnit.RestoreMovement();
                currentUnit.RestoreActionPoint();
                currentUnit.RecoverFatigue();

                var newTurn = new LevelEvent.NewTurn(currentUnit);
                WriteEvent(newTurn);

                while (true)
                {
                    // ReSharper restore RedundantAssignment
                    var command = await WaitForAgentInput(currentUnit);
                    bool isDone;
                    switch (command)
                    {
                        case ActionCommand actionCommand:
                            await ActionEvaluationFlow(actionCommand, currentUnit);
                            isDone = false;
                            break;
                        case EndTurnCommand:
                        case NullCommand:
                            isDone = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(command));
                    }

                    if (isDone) break;
                }

                turnNum++;
                UnitTurnOrderer.ToNextUnit();
            }

            // Expected, this should just run forever, until we have a exit/win/lose condition
            // ReSharper disable once FunctionNeverReturns
        }

        public void ForcePlayEvent(LevelEvent levelEvent)
        {
            WriteEvent(levelEvent);
        }

        private void WriteEvent(LevelEvent levelEvent)
        {
            if (!_eventChannel.Writer.TryWrite(levelEvent)) Log.Error("[Level] You should really, never have got here");
        }

        private async UniTask ActionEvaluationFlow(ActionCommand command, UnitData currentUnit)
        {
            EvaluateCommand(command);
            await ComboFlow(command, currentUnit);
        }

        private void EvaluateCommand(ActionCommand command)
        {
            var actionSequence = Evaluator.Evaluate(command).ToArray();
            var sequenceEvent = new LevelEvent.SequenceOccured(actionSequence);
            WriteEvent(sequenceEvent);
        }

        private async UniTask ComboFlow(ActionCommand startingCommand, UnitData comboStarter)
        {
            // no combo -> nothing to do we can just bugger off
            var possibleCombos = _comboChecker.FindComboOptions(startingCommand).ToArray();
            if (!possibleCombos.Any()) return;

            // wait till the agent give us to something to work on 
            var comboDecisionEvent = new LevelEvent.WaitForComboDecision(comboStarter, possibleCombos);
            WriteEvent(comboDecisionEvent);
            if (await WaitForAgentInput(comboStarter) is not ActionCommand actionCommand) return;

            // work on that something
            EvaluateCommand(actionCommand);

            // check if we can actually keep comboing -> combo till heat death if necessary
            if (actionCommand.ActorEntity is not UnitData comboTaker) return;
            await ComboFlow(actionCommand, comboTaker);
        }

        private async UniTask<ICommand> WaitForAgentInput(UnitData currentUnit)
        {
            IDecisionValidator.Error? err;
            ICommand command;
            do
            {
                var decision = await AgentsService.GetAgentDecision(currentUnit.FactionId);
                Log.Info($"[Level] Received Decision: {decision?.GetType()}");

                (err, command) = DecisionValidator.ValidateDecision(decision);

                if (err != null) Log.Info($"[Level] Decision Error: {err.Id}, {err.Description}");
            } while (err != null);

            Log.Info($"[Level] Evaluate Command: {command.GetType()}");

            //TODO: preferrably there is an auto end turn button which ends turn when there's absolutely nothing you can do, hard to implement without being annoying though
            return command;
        }
    }
}
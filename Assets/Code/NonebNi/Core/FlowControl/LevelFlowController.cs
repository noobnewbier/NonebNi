using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Agents;
using NonebNi.Core.Combos;
using NonebNi.Core.Commands;
using NonebNi.Core.Effects;
using NonebNi.Core.Units;
using Unity.Logging;

namespace NonebNi.Core.FlowControl
{
    public interface ILevelFlowController
    {
        void Run();
        void ForcePlayEvent(LevelEvent levelEvent);

        #region Editor Console's dependencies

        //todo: at some point, it would be brill if we can get rid of these, for now though.

        IAgentsService AgentsService { get; }
        IActionCommandEvaluator Evaluator { get; }
        IUnitTurnOrderer UnitTurnOrderer { get; }

        #endregion
    }

    //todo: next big work - combo
    //todo: combo ui - might be able to reuse quite a bit.

    //todo: could use an error message panel/tooltip even just for debugging
    //todo: fatigue work upwards only in the ui - it's easier to keep the rest of the maths consistent(going downwards)
    //todo: action point system? make it play with speed or sth idk
    //todo: fix I can unintentionally control enemy - (or maybe keep it but make sure it's debug view)
    //todo: fix order panel
    //todo: fix starting unit
    //todo: fix initiative - or leave it as is and be happy with it.
    //todo: enemy ai, it can be simple raider/wolf ai
    //todo: at some point we need save/load test.
    //todo: show cost of an action, either tooltip whatever easiest solution you can come up with.
    //todo: at some point a better logging system would be nice - but not necessary...?
    public class LevelFlowController : ILevelFlowController
    {
        public LevelFlowController(
            IActionCommandEvaluator evaluator,
            IUnitTurnOrderer unitTurnOrderer,
            IAgentsService agentService,
            IComboChecker comboChecker,
            IGameEventControl gameEventControl)
        {
            Evaluator = evaluator;
            UnitTurnOrderer = unitTurnOrderer;
            AgentsService = agentService;
            _comboChecker = comboChecker;
            _gameEventControl = gameEventControl;
        }

        public IAgentsService AgentsService { get; }
        public IActionCommandEvaluator Evaluator { get; }
        public IUnitTurnOrderer UnitTurnOrderer { get; }

        private readonly IGameEventControl _gameEventControl;
        private readonly IComboChecker _comboChecker;

        public void Run()
        {
            //todo: at some point we need a way to kill it.
            RunLevelFlow().Forget();
        }

        private async UniTask RunLevelFlow()
        {
            _gameEventControl.WriteEvent(new LevelEvent.GameStart());

            //TODO: replace all these logging w/ a Decorator using StrongInject.

            var turnNum = 0; //Mostly for debug purposes - but probably necessary for UI at some point
            while (true)
            {
                var currentUnit = UnitTurnOrderer.CurrentUnit;
                Log.Info($"[Level] Turn {turnNum}, {currentUnit.Name}'s turn");

                currentUnit.RestoreMovement();
                currentUnit.RestoreActionPoint();
                currentUnit.RecoverFatigue();

                while (true)
                {
                    var waitForUnitDecision = new LevelEvent.WaitForActiveUnitDecision(currentUnit);
                    _gameEventControl.WriteEvent(waitForUnitDecision);

                    // ReSharper restore RedundantAssignment
                    var command = await AgentsService.GetAgentInput(currentUnit.FactionId);
                    var isDone = false;
                    var unitKeepActing = false;
                    switch (command)
                    {
                        case ActionCommand actionCommand:
                            await ActionEvaluationFlow(actionCommand, currentUnit);
                            unitKeepActing = true;
                            break;
                        case EndTurnCommand:
                        case NullCommand:
                            isDone = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(command));
                    }

                    if (unitKeepActing) continue;
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
            _gameEventControl.WriteEvent(levelEvent);
        }

        private async UniTask ActionEvaluationFlow(ActionCommand command, UnitData currentUnit)
        {
            var context = EvaluateCommand(command);
            await ComboFlow(context, currentUnit);
        }

        private EffectContext EvaluateCommand(ActionCommand command)
        {
            var context = Evaluator.FindEffectContext(command);
            var actionSequence = Evaluator.Evaluate(command).ToArray();
            var sequenceEvent = new LevelEvent.SequenceOccured(actionSequence);
            _gameEventControl.WriteEvent(sequenceEvent);

            return context;
        }

        private async UniTask ComboFlow(EffectContext comboContext, UnitData comboStarter)
        {
            // no combo -> nothing to do we can just bugger off
            var possibleCombos = _comboChecker.FindComboOptions(comboContext).ToArray();
            if (!possibleCombos.Any()) return;

            // wait till the agent give us to something to work on 
            var comboDecisionEvent = new LevelEvent.WaitForComboDecision(comboStarter, possibleCombos);
            _gameEventControl.WriteEvent(comboDecisionEvent);
            if (await AgentsService.GetAgentInput(comboStarter.FactionId) is not ActionCommand actionCommand) return;

            // work on that something
            var nextComboContext = EvaluateCommand(actionCommand);

            // check if we can actually keep comboing -> combo till heat death if necessary
            if (actionCommand.ActorEntity is not UnitData comboTaker) return;
            await ComboFlow(nextComboContext, comboTaker);
        }
    }
}
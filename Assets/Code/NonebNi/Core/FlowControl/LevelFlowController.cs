using Cysharp.Threading.Tasks;
using NonebNi.Core.Agents;
using NonebNi.Core.Commands;
using NonebNi.Core.Decisions;
using NonebNi.Core.Sequences;
using NonebNi.Core.Units;
using Unity.Logging;

namespace NonebNi.Core.FlowControl
{
    public interface ILevelFlowController
    {
        delegate void TurnStarted(UnitData currentUnit);

        IAgentsService AgentsService { get; }
        ICommandEvaluationService EvaluationService { get; }
        ISequencePlayer SequencePlayer { get; }
        IUnitTurnOrderer UnitTurnOrderer { get; }
        UniTask Run();
        event TurnStarted NewTurnStarted;
    }

    public class LevelFlowController : ILevelFlowController
    {
        public LevelFlowController(
            ICommandEvaluationService evaluationService,
            IUnitTurnOrderer unitTurnOrderer,
            IAgentsService agentService,
            ISequencePlayer sequencePlayer,
            IDecisionValidator decisionValidator)
        {
            EvaluationService = evaluationService;
            UnitTurnOrderer = unitTurnOrderer;
            AgentsService = agentService;
            SequencePlayer = sequencePlayer;
            DecisionValidator = decisionValidator;
        }

        public IDecisionValidator DecisionValidator { get; }

        public IAgentsService AgentsService { get; }

        public ICommandEvaluationService EvaluationService { get; }

        public ISequencePlayer SequencePlayer { get; }

        public IUnitTurnOrderer UnitTurnOrderer { get; }

        public async UniTask Run()
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

                NewTurnStarted?.Invoke(currentUnit);

                // ReSharper disable RedundantAssignment - Can't declare value tuple without assigning
                (IDecisionValidator.Error? err, var command) = (null, NullCommand.Instance);
                // ReSharper restore RedundantAssignment
                do
                {
                    var decision = await AgentsService.GetAgentDecision(currentUnit.FactionId);
                    Log.Info($"[Level] Received Decision: {decision?.GetType()}");

                    (err, command) = DecisionValidator.ValidateDecision(decision);

                    if (err != null) Log.Info($"[Level] Decision Error: {err.Id}, {err.Description}");
                } while (err != null);


                Log.Info($"[Level] Evaluate Command: {command.GetType()}");
                //TODO: preferrably there is an auto end turn button which ends turn when there's absolutely nothing you can do, hard to implement without being annoying though
                if (command is EndTurnCommand)
                {
                    turnNum++;
                    UnitTurnOrderer.ToNextUnit();
                }
                else
                {
                    var sequences = EvaluationService.Evaluate(command);
                    await SequencePlayer.Play(sequences);
                }

                Log.Info("[Level] Finished Evaluation");
            }

            // Expected, this should just run forever, until we have a exit/win/lose condition
            // ReSharper disable once FunctionNeverReturns
        }

        public event ILevelFlowController.TurnStarted? NewTurnStarted;
    }
}
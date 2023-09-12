using Cysharp.Threading.Tasks;
using NonebNi.Core.Agents;
using NonebNi.Core.Commands;
using NonebNi.Core.Decisions;
using NonebNi.Core.Sequences;
using UnityEngine;

namespace NonebNi.Core.FlowControl
{
    public interface ILevelFlowController
    {
        IAgentsService AgentsService { get; }
        ICommandEvaluationService EvaluationService { get; }
        ISequencePlayer SequencePlayer { get; }
        IUnitTurnOrderer UnitTurnOrderer { get; }
        UniTask Run();
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
                Debug.Log($"[Level] Turn {turnNum}, {currentUnit.Name}'s turn");

                // ReSharper disable RedundantAssignment - Can't declare value tuple without assigning
                var (err, command) = (default(IDecisionValidator.Error), NullCommand.Instance);
                // ReSharper restore RedundantAssignment
                do
                {
                    var decision = await AgentsService.GetAgentDecision(currentUnit.FactionId);
                    Debug.Log($"[Level] Received Decision: {decision?.GetType()}");

                    (err, command) = DecisionValidator.ValidateDecision(decision);

                    if (err != null) Debug.Log($"[Level] Decision Error: {err.Id}, {err.Description}");
                } while (err != null);


                Debug.Log($"[Level] Evaluate Command: {command.GetType()}");
                var sequences = EvaluationService.Evaluate(command);
                await SequencePlayer.Play(sequences).ToUniTask();

                turnNum++;
                UnitTurnOrderer.ToNextUnit();
                Debug.Log("[Level] Finished Evaluation");
            }

            // Expected, this should just run forever, until we have a exit/win/lose condition
            // ReSharper disable once FunctionNeverReturns
        }
    }
}
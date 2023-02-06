using Cysharp.Threading.Tasks;
using NonebNi.Core.Agents;
using NonebNi.Core.Commands;
using NonebNi.Core.Decision;
using NonebNi.Core.Sequences;
using UnityEngine;

namespace NonebNi.Core.FlowControl
{
    public interface ILevelFlowController
    {
        UniTask Run();
    }

    public class LevelFlowController : ILevelFlowController
    {
        private readonly IAgentsService _agentService;
        private readonly IDecisionValidator _decisionValidator;
        private readonly ICommandEvaluationService _evaluationService;
        private readonly ISequencePlayer _sequencePlayer;
        private readonly IUnitTurnOrderer _unitTurnOrderer;

        public LevelFlowController(ICommandEvaluationService evaluationService,
            IUnitTurnOrderer unitTurnOrderer,
            IAgentsService agentService,
            ISequencePlayer sequencePlayer,
            IDecisionValidator decisionValidator)
        {
            _evaluationService = evaluationService;
            _unitTurnOrderer = unitTurnOrderer;
            _agentService = agentService;
            _sequencePlayer = sequencePlayer;
            _decisionValidator = decisionValidator;
        }

        public async UniTask Run()
        {
            //TODO: replace all these logging w/ a Decorator using StrongInject.

            var turnNum = 0; //Mostly for debug purposes - but probably necessary for UI at some point
            while (true)
            {
                var currentUnit = _unitTurnOrderer.CurrentUnit;
                Debug.Log($"[Level] Turn {turnNum}, {currentUnit.Name}'s turn");

                // ReSharper disable RedundantAssignment - Can't declare value tuple without assigning
                var (err, command) = (default(IDecisionValidator.Error), NullCommand.Instance);
                // ReSharper restore RedundantAssignment
                do
                {
                    var decision = await _agentService.GetAgentDecision(currentUnit.FactionId);
                    Debug.Log($"[Level] Received Decision: {decision?.GetType()}");

                    (err, command) = _decisionValidator.ValidateDecision(decision);

                    if (err != null)
                    {
                        Debug.Log($"[Level] Decision Error: {err.Id}, {err.Description}");
                    }
                } while (err != null);


                Debug.Log($"[Level] Evaluate Command: {command.GetType()}");
                var sequences = _evaluationService.Evaluate(command);
                await _sequencePlayer.Play(sequences).ToUniTask();

                turnNum++;
                _unitTurnOrderer.ToNextUnit();
                Debug.Log($"[Level] Finished Evaluation");
            }
        }
    }
}
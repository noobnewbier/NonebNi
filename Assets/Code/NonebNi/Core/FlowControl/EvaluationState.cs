using System.Collections;
using NonebNi.Core.Sequences;
using NonebNi.Core.StateMachines;
using UnityEngine;

namespace NonebNi.Core.FlowControl
{
    public class EvaluationState : IState
    {
        private readonly ICommandEvaluationService _evaluationService;
        private readonly ILevelFlowController _levelFlowController;
        private readonly IPlayerDecisionService _playerDecisionService;
        private readonly ISequencePlayer _sequencePlayer;

        public EvaluationState(ICommandEvaluationService evaluationService,
            ILevelFlowController levelFlowController,
            IPlayerDecisionService playerDecisionService,
            ISequencePlayer sequencePlayer)
        {
            _evaluationService = evaluationService;
            _levelFlowController = levelFlowController;
            _playerDecisionService = playerDecisionService;
            _sequencePlayer = sequencePlayer;
        }

        public IEnumerator OnUpdate()
        {
            Debug.Log("[Evaluate] Update");

            yield break;
        }

        public IEnumerator OnEnterState()
        {
            Debug.Log("[Evaluate] Enter");

            var sequences = _evaluationService.Evaluate(_playerDecisionService.Command);

            yield return _sequencePlayer.Play(sequences);

            _levelFlowController.FinishEvaluation();
        }

        public IEnumerator OnExitState()
        {
            Debug.Log("[Evaluate] Exit");

            yield break;
        }
    }
}
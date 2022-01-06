using NonebNi.Core.StateMachines;
using UnityEngine;

namespace NonebNi.Core.FlowControl
{
    public class EvaluationState : IState
    {
        private readonly ICommandEvaluationService _evaluationService;
        private readonly ILevelFlowController _levelFlowController;
        private readonly IPlayerDecisionService _playerDecisionService;

        public EvaluationState(ICommandEvaluationService evaluationService,
                               ILevelFlowController levelFlowController,
                               IPlayerDecisionService playerDecisionService)
        {
            _evaluationService = evaluationService;
            _levelFlowController = levelFlowController;
            _playerDecisionService = playerDecisionService;
        }

        public void OnUpdate()
        {
            Debug.Log("[Evaluate] Update");
        }

        public void OnEnterState()
        {
            Debug.Log("[Evaluate] Enter");

            _evaluationService.Evaluate(_playerDecisionService.Command);
            _levelFlowController.FinishEvaluation();
        }

        public void OnExitState()
        {
            Debug.Log("[Evaluate] Exit");
        }
    }
}
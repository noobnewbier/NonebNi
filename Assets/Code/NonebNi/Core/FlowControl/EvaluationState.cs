using NonebNi.Core.StateMachines;
using UnityEngine;

namespace NonebNi.Core.FlowControl
{
    public class EvaluationState : IState
    {
        private readonly ICommandEvaluationService _evaluationService;
        private readonly ILevelFlowController _levelFlowController;

        public EvaluationState(ICommandEvaluationService evaluationService, ILevelFlowController levelFlowController)
        {
            _evaluationService = evaluationService;
            _levelFlowController = levelFlowController;
        }

        public void OnUpdate()
        {
            Debug.Log("[Evaluate] Update");
        }

        public void OnEnterState()
        {
            Debug.Log("[Evaluate] Enter");

            _evaluationService.Evaluate();
            _levelFlowController.FinishEvaluation();
        }

        public void OnExitState()
        {
            Debug.Log("[Evaluate] Exit");
        }
    }
}
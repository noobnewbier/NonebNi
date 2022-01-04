using NonebNi.Core.StateMachines;
using UnityEngine;

namespace NonebNi.Core.FlowControl
{
    public class DecisionState : IState
    {
        private readonly IUnitTurnOrderer _unitTurnOrderer;

        public DecisionState(IUnitTurnOrderer unitTurnOrderer)
        {
            _unitTurnOrderer = unitTurnOrderer;
        }

        public void OnUpdate()
        {
            Debug.Log("[Decision] Update");
        }

        public void OnEnterState()
        {
            var currentUnit = _unitTurnOrderer.ToNextUnit();

            Debug.Log($"[Decision] Enter - {currentUnit.Name}");
        }

        public void OnExitState()
        {
            Debug.Log("[Decision] Exit");
        }
    }
}
using System.Collections;
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

        public IEnumerator OnUpdate()
        {
            Debug.Log("[Decision] Update");

            yield break;
        }

        public IEnumerator OnEnterState()
        {
            var currentUnit = _unitTurnOrderer.ToNextUnit();

            Debug.Log($"[Decision] Enter - {currentUnit.Name}");

            yield break;
        }

        public IEnumerator OnExitState()
        {
            Debug.Log("[Decision] Exit");

            yield break;
        }
    }
}
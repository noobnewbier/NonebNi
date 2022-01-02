using NonebNi.Core.StateMachines;
using UnityEngine;

namespace NonebNi.Core.FlowControl
{
    public class DecisionState : IState
    {
        public void OnUpdate()
        {
            Debug.Log("[Decision] Update");
        }

        public void OnEnterState()
        {
            Debug.Log("[Decision] Enter");
        }

        public void OnExitState()
        {
            Debug.Log("[Decision] Exit");
        }
    }
}
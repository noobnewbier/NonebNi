using NonebNi.Core.StateMachines;
using UnityEngine;

namespace NonebNi.Core.FlowControl
{
    public class EndState : IState
    {
        public void OnUpdate()
        {
            Debug.Log("[End] Update");
        }

        public void OnEnterState()
        {
            Debug.Log("[End] Enter");
        }

        public void OnExitState()
        {
            Debug.Log("[End] Exit");
        }
    }
}
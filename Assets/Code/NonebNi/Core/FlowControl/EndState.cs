using System.Collections;
using NonebNi.Core.StateMachines;
using UnityEngine;

namespace NonebNi.Core.FlowControl
{
    public class EndState : IState
    {
        public IEnumerator OnUpdate()
        {
            Debug.Log("[End] Update");

            yield break;
        }

        public IEnumerator OnEnterState()
        {
            Debug.Log("[End] Enter");

            yield break;
        }

        public IEnumerator OnExitState()
        {
            Debug.Log("[End] Exit");

            yield break;
        }
    }
}
using System.Collections;

namespace NonebNi.Core.StateMachines
{
    public interface IState
    {
        IEnumerator OnUpdate();

        IEnumerator OnEnterState();

        IEnumerator OnExitState();
    }
}
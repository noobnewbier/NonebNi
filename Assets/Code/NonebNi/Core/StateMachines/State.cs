namespace NonebNi.Core.StateMachines
{
    public interface IState
    {
        void OnUpdate();

        void OnEnterState();

        void OnExitState();
    }
}
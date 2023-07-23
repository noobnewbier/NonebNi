using System.Collections.Generic;

namespace NonebNi.Core.StateMachines
{
    public class Transition
    {
        public Transition(ISet<string> parameters, IState targetState, int priority = 0)
        {
            Parameters = parameters;
            TargetState = targetState;
            Priority = priority;
        }

        public ISet<string> Parameters { get; }
        public IState TargetState { get; }

        /// <summary>
        ///     Priority is ordered inversely, i.e lower Priority value is prioritize
        /// </summary>
        public int Priority { get; }
    }
}
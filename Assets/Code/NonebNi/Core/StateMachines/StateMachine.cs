using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable PossibleMultipleEnumeration

namespace NonebNi.Core.StateMachines
{
    /// <summary>
    ///     Basically mimicking what Unity does with their Animator, without actually relying on it.
    ///     When multiple transition can be used, the one with the highest priority is chosen
    ///     Note:
    ///     Atm we only support triggers, as it is the easiest one to implement and we don't actually need other kind of parameters
    ///     for
    ///     now.
    ///     Shall the need arise we will refactor.
    /// </summary>
    public class StateMachine
    {
        private readonly IState _defaultState;
        private readonly Dictionary<IState, Transition[]> _sourceStateAndTransitions;
        private readonly Dictionary<string, BooleanWrapper> _triggers;

        private IState? _currentState;

        public StateMachine(IState defaultState)
        {
            _defaultState = defaultState;
            _triggers = new Dictionary<string, BooleanWrapper>();
            _sourceStateAndTransitions = new Dictionary<IState, Transition[]>();
        }

        public IEnumerator UpdateState()
        {
            var nextState = GetNextState();
            if (nextState != _currentState)
            {
                yield return _currentState?.OnExitState();
                yield return nextState.OnEnterState();

                _currentState = nextState;
            }
            else
            {
                yield return _currentState.OnUpdate();
            }
        }

        public void AddTransition(IState sourceState, params Transition[] transitions)
        {
            if (!_sourceStateAndTransitions.ContainsKey(sourceState)) _sourceStateAndTransitions[sourceState] = transitions;
            else
                _sourceStateAndTransitions[sourceState] = _sourceStateAndTransitions[sourceState]
                    .Concat(transitions)
                    .OrderBy(t => t.Priority)
                    .ThenByDescending(t => t.Parameters.Count)
                    .ToArray();


            foreach (var parameter in transitions.SelectMany(t => t.Parameters).Distinct())
                if (!_triggers.ContainsKey(parameter))
                    _triggers[parameter] = new BooleanWrapper();
        }

        public void SetTrigger(string key)
        {
            if (!_triggers.ContainsKey(key))
                throw new InvalidOperationException($@"""{key}"" is undefined in any transition");

            _triggers[key].Value = true;
        }

        private IState GetNextState()
        {
            if (_currentState == null) return _defaultState;

            //assuming Transition is ordered in priority
            if (_sourceStateAndTransitions.ContainsKey(_currentState))
            {
                var transitions = _sourceStateAndTransitions[_currentState];
                foreach (var transition in transitions)
                {
                    var parameters = transition.Parameters;
                    var triggers = _triggers.Where(p => parameters.Contains(p.Key)).Select(p => p.Value);

                    var allParametersAreFound = triggers.Count() == parameters.Count;
                    if (triggers.All(v => v) && allParametersAreFound)
                    {
                        void ResetTriggers()
                        {
                            foreach (var wrapper in triggers) wrapper.Value = false;
                        }

                        ResetTriggers();
                        return transition.TargetState;
                    }
                }
            }

            return _currentState;
        }

        private class BooleanWrapper
        {
            public bool Value { get; set; }

            public static implicit operator bool(BooleanWrapper wrapper) => wrapper.Value;
        }
    }
}
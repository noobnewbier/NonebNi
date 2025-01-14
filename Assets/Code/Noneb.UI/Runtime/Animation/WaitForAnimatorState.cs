using UnityEngine;

namespace NonebNi.Ui.Common
{
    public class WaitForAnimatorState : CustomYieldInstruction
    {
        private readonly Animator _animator;
        private readonly int _layer;
        private readonly string _targetState;

        public WaitForAnimatorState(Animator animator, int layer, string targetState)
        {
            _animator = animator;
            _layer = layer;
            _targetState = targetState;
        }

        public override bool keepWaiting
        {
            get
            {
                var currentState = _animator.GetCurrentAnimatorStateInfo(_layer);
                if (!currentState.IsName(_targetState)) return true;

                return currentState.normalizedTime >= 1;
            }
        }
    }
}
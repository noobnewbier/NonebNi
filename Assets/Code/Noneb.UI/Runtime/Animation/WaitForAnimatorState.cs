using UnityEngine;

namespace Noneb.UI.Animation
{
    public class WaitForAnimatorState : CustomYieldInstruction
    {
        private readonly Animator _animator;
        private readonly int _layer;
        private readonly AnimatorStateInfo _startingState;
        private readonly string _targetStateNam;
        private bool _arrivedTargetState;

        public WaitForAnimatorState(Animator animator, int layer, string targetStateNam)
        {
            _animator = animator;
            _startingState = animator.GetCurrentAnimatorStateInfo(_layer);
            _layer = layer;
            _targetStateNam = targetStateNam;
        }

        public override bool keepWaiting
        {
            get
            {
                var currentState = _animator.GetCurrentAnimatorStateInfo(_layer);

                // exited the target state already and we didn't catch it happen -> stop waiting
                if (_arrivedTargetState && currentState.shortNameHash != _startingState.shortNameHash) return false;

                // yay - arrived target state
                if (currentState.IsName(_targetStateNam))
                {
                    _arrivedTargetState = true;

                    // if target state is still playing -> keep waiting
                    return currentState.normalizedTime < 1;
                }

                // Still in transition or whatever, keep waiting
                return true;
            }
        }
    }
}
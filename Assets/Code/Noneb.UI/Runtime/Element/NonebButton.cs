using System;
using Unity.Logging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Noneb.UI.Element
{
    /// <summary>
    /// TODO: there will be more to do regarding UI state transition/animation but for now this is good enough.
    /// </summary>
    public class NonebButton : Selectable, IElementComponent, IPointerClickHandler, ISubmitHandler
    {
        protected override void Awake()
        {
            base.Awake();

            if (this.FindOwner() == null) Log.Error($"Cannot find owner of NonebButton in {gameObject.name} - likely unintended");
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();
        }

        public event Action? Clicked;

        private void Press()
        {
            DoStateTransition(SelectionState.Pressed, false);
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            Clicked?.Invoke();
        }


        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (!gameObject.activeInHierarchy)
                return;

            if (transition is Transition.ColorTint or Transition.SpriteSwap)
            {
                Log.Error("I don't want to support that it's rarely useful");
                return;
            }

            var isActivated = this.FindOwner()?.IsElementActive == true;
            if (!isActivated)
            {
                PlayTriggerAnimation(animationTriggers.normalTrigger);
                return;
            }

            SetDisabledState(state);
            var triggerName = state switch
            {
                SelectionState.Normal => animationTriggers.normalTrigger,

                //TODO: more sophisticated logic on the press bit, otherwise animation won't look nice
                //this includes - move away from press, detecting press release so we can get out of the pressed state, yada yada yaddaa
                SelectionState.Pressed => animationTriggers.pressedTrigger,

                //These two are essentially the same.
                SelectionState.Highlighted => animationTriggers.highlightedTrigger,
                SelectionState.Selected => animationTriggers.selectedTrigger,
                _ => string.Empty
            };

            PlayTriggerAnimation(triggerName);
        }

        private void SetDisabledState(SelectionState state)
        {
            /*
             * By convention this is never a trigger, as I want the player to be able to hover/click/press on a disabled button.
             * I don't want to rewrite the whole selectable thing which would be in a way impossible as I can't easily modify the source class without some magic that I want to avoid.
             *
             * I can live with the editor quirks, although I will hate Unity with a passion.
             */
            animator.SetBool(animationTriggers.disabledTrigger, state == SelectionState.Disabled);
        }

        private void PlayTriggerAnimation(string triggerName)
        {
            if (transition != Transition.Animation || animator == null || !animator.isActiveAndEnabled || !animator.hasBoundPlayables || string.IsNullOrEmpty(triggerName))
                return;

            animator.ResetTrigger(animationTriggers.normalTrigger);
            animator.ResetTrigger(animationTriggers.highlightedTrigger);
            animator.ResetTrigger(animationTriggers.pressedTrigger);
            animator.ResetTrigger(animationTriggers.selectedTrigger);

            animator.SetTrigger(triggerName);
        }
    }
}
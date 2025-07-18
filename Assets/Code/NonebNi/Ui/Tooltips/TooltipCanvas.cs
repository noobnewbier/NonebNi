using System;
using Cysharp.Threading.Tasks;
using Noneb.UI.InputSystems;
using Noneb.UI.View;
using NonebNi.Ui.UIContexts;
using UnityEngine;
using UnityUtils.Pooling;

namespace NonebNi.Ui.Tooltips
{
    public interface ITooltipCanvas : IViewComponent
    {
        void RequestTooltip(TooltipRequest request);
        UniTask HideCurrentTooltip();
    }

    public class TooltipCanvas : MonoBehaviour, ITooltipCanvas
    {
        [SerializeField] private TextTooltip textTooltipTemplate = null!;

        private TooltipData? _currentRec;
        private Dependencies _deps = null!;

        private BehaviourPool<TextTooltip> _textTooltipPool = null!;

        private void Update()
        {
            UpdateTooltipPos();
        }

        public async UniTask OnViewInit()
        {
            _textTooltipPool = new BehaviourPool<TextTooltip>(textTooltipTemplate);
            _deps = await UIContext.Get<Dependencies>();
        }

        public UniTask OnViewTearDown()
        {
            _textTooltipPool.Dispose();
            return UniTask.CompletedTask;
        }

        public UniTask OnViewLeave(INonebView currentView, INonebView? nextView)
        {
            HideCurrentTooltip().Forget();
            return UniTask.CompletedTask;
        }

        public void RequestTooltip(TooltipRequest request)
        {
            switch (request)
            {
                case TooltipRequest.Text textRequest:
                    var pooledObject = _textTooltipPool.Get(out var tooltip);
                    var data = new TooltipData(pooledObject, tooltip);
                    ShowTooltip(data, textRequest).Forget();
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(request));
            }
        }

        public async UniTask HideCurrentTooltip()
        {
            if (_currentRec == null) return;

            var oldRec = _currentRec;
            var task = oldRec.TooltipInstance.Hide();
            // logic needs to happen before the task is completed. 
            _currentRec = null;

            await task;
            oldRec.PoolObj.Dispose();
        }

        private async UniTask ShowTooltip(TooltipData rec, TooltipRequest request)
        {
            HideCurrentTooltip().Forget();
            _currentRec = rec;

            /*
             * Currently we only position our tooltip around the cursor.
             * In the future we might want a more sophisticated solution where we are taking UI element/game object into account as well.
             * But for now, this is "good enough".
             */


            UpdateTooltipPos();
            await _currentRec.TooltipInstance.Show(request);
        }

        private void UpdateTooltipPos()
        {
            if (_currentRec == null) return;

            // prioritize top -> right, if we can't go left and bottom
            var cursorPos = _deps.InputSystem.MousePosition;
            _currentRec.TooltipInstance.transform.SetParent(transform);
            _currentRec.TooltipInstance.transform.position = cursorPos;
        }

        /// <summary>
        /// I needed a way to maintain reference to both the PooledObject and the TooltipBehaviour.
        /// Otherwise, we would need to do weird type checking to find out which pool the behaviour belongs to and that was too
        /// much work for me.
        /// Not sure how to name it better, future me that's your call.
        /// </summary>
        private record TooltipData(IDisposable PoolObj, TooltipBehaviour TooltipInstance);

        public record Dependencies(IInputSystem InputSystem) : ISharedInContext;
    }
}
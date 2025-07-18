using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.InputSystems;
using Noneb.UI.View;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Ui.Tooltips;
using NonebNi.Ui.UIContexts;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using UnityEngine;

namespace NonebNi.Ui.ViewComponents.HexTooltip
{
    public interface IHexTooltipControl : IViewComponent<IHexTooltipControl.Data>
    {
        public record Data;
    }

    /// <summary>
    /// Would like consistent naming for this kind of stuffs.
    /// </summary>
    public class HexTooltipControl : MonoBehaviour, IHexTooltipControl
    {
        [SerializeField] private float interruptThreshold = 1f;
        [SerializeField] private float dismissThreshold = 1f;
        [SerializeField] private float delay = 0.75f;
        private CancellationTokenSource _cts = new();

        private Dependencies _deps = null!;

        private UniTask _task;

        public async UniTask OnViewInit()
        {
            _deps = await UIContext.Get<Dependencies>();
        }

        public UniTask OnViewEnter(INonebView? previousView, INonebView currentView)
        {
            _cts = new CancellationTokenSource();
            _task = Run(_cts.Token);

            return UniTask.CompletedTask;
        }

        public async UniTask OnViewLeave(INonebView currentView, INonebView? nextView)
        {
            _cts.Cancel();
            await _task;
        }

        private async UniTask Run(CancellationToken ct)
        {
            var isShowing = false;
            var lastCursorPos = _deps.InputSystem.MousePosition;
            var lastCoord = _deps.WorldSpaceInputControl.FindHoveredCoordinate();
            var timer = 0f;
            while (!ct.IsCancellationRequested)
            {
                await UniTask.Yield(ct, true);

                var nextCursorPos = _deps.InputSystem.MousePosition;
                var nextCoord = _deps.WorldSpaceInputControl.FindHoveredCoordinate();
                var dist = Vector3.Distance(lastCursorPos, nextCursorPos);

                if (isShowing)
                {
                    if (dist > dismissThreshold || lastCoord != nextCoord)
                    {
                        isShowing = false;
                        await _deps.TooltipCanvas.HideCurrentTooltip();
                    }
                }
                else
                {
                    if (dist > interruptThreshold || lastCoord != nextCoord || lastCoord == null)
                        timer = 0f;
                    else
                        timer += Time.deltaTime;

                    if (timer > delay)
                        if (lastCoord != null)
                        {
                            var tile = _deps.Map.Get(lastCoord);
                            var entity = _deps.Map.Get<EntityData>(lastCoord);

                            var text = $"This is a {tile.Name} tile.";
                            if (entity != null) text += $"It has a {entity.Name} on it.";

                            var tooltip = new TooltipRequest.Text(text);
                            _deps.TooltipCanvas.RequestTooltip(tooltip);

                            isShowing = true;
                        }
                }

                lastCursorPos = nextCursorPos;
                lastCoord = nextCoord;
            }

            if (isShowing) _deps.TooltipCanvas.HideCurrentTooltip();
        }

        public record Dependencies(ITooltipCanvas TooltipCanvas, IInputSystem InputSystem, IPlayerTurnWorldSpaceInputControl WorldSpaceInputControl, IReadOnlyMap Map) : ISharedInContext;
    }
}
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.Element;
using NonebNi.Ui.Attributes;
using NonebNi.Ui.UIContexts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NonebNi.Ui.Tooltips
{
    /// <summary>
    /// Copied and adapted from:
    /// https://github.com/Unity-Technologies/com.unity.multiplayer.samples.coop/blob/main/Assets/Scripts/Gameplay/UI/UITooltipDetector.cs#L46
    /// Attach to any UI element that should have a tooltip popup. If the mouse hovers over this element
    /// long enough, the tooltip will appear and show the specified text.
    /// </summary>
    /// <remarks>
    /// Having trouble getting the tooltips to show up? The event-handlers use physics raycasting, so make sure:
    /// - the main camera in the scene has a PhysicsRaycaster component
    /// - if you're attaching this to a UI element such as an Image, make sure you check the "Raycast Target" checkbox
    /// </remarks>
    public class TooltipDetector : MonoBehaviour, IElementComponent, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeReference, TypePicker, Tooltip("The actual Tooltip that should be triggered")] private TooltipRequest? request;

        [SerializeField, Tooltip("Should the tooltip appear instantly if the player clicks this UI element?")] private bool activateOnClick = true;

        [SerializeField, Tooltip("The length of time the mouse needs to hover over this element before the tooltip appears (in seconds)")] private float delay = 0.5f;

        private Dependencies? _deps;
        private bool _isShowingTooltip;
        private float _pointerEnterTime;

        private void Update()
        {
            if (!this.IsElementActive()) return;

            if (_pointerEnterTime != 0 && Time.time - _pointerEnterTime > delay) ShowTooltip();
        }

        public async UniTask OnInit(CancellationToken ct)
        {
            _deps = await UIContext.Get<Dependencies>(ct);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!this.IsElementActive()) return;

            if (!activateOnClick) return;

            ShowTooltip();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!this.IsElementActive()) return;

            _pointerEnterTime = Time.time;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!this.IsElementActive()) return;

            _pointerEnterTime = 0;
            HideTooltip();
        }

        public void SetRequest(TooltipRequest newRequest)
        {
            var wasChanged = request != newRequest;
            request = newRequest;
            if (wasChanged && _isShowingTooltip)
            {
                // we changed the text while of our tooltip was being shown! We need to re-show the tooltip!
                HideTooltip();
                ShowTooltip();
            }
        }

        private void ShowTooltip()
        {
            if (_isShowingTooltip) return;

            if (request == null) return;

            _deps?.TooltipCanvas.RequestTooltip(request);
            _isShowingTooltip = true;
        }

        private void HideTooltip()
        {
            if (!_isShowingTooltip) return;

            _deps?.TooltipCanvas.HideCurrentTooltip();
            _isShowingTooltip = false;
        }

        public record Dependencies(ITooltipCanvas TooltipCanvas) : ISharedInContext;
    }
}
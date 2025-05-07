using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.Animation;
using Noneb.UI.Element;
using NonebNi.Core.Actions;
using TMPro;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    public class ActionOption : MonoBehaviour, IElementComponent
    {
        [SerializeField] private TextMeshProUGUI actionName = null!;
        [SerializeField] private NonebButton button = null!;

        [SerializeField] private AnimationData highlightOnAnim = new();
        [SerializeField] private AnimationData highlightOffAnim = new();

        public NonebAction? Action { get; private set; }

        private void Awake()
        {
            button.Clicked += OnClick;
        }


        public event Action<NonebAction>? Clicked;

        public async UniTask Show(NonebAction action, CancellationToken ct = default)
        {
            Action = action;
            actionName.text = action.Name.GetLocalized();
        }

        private void OnClick()
        {
            if (Action == null)
            {
                Log.Error("Clicked before showing anything -> probably unintended sequence");
                return;
            }

            Clicked?.Invoke(Action);
        }

        public async UniTask SetHighlight(bool isHighlighted, CancellationToken ct = default)
        {
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);

            var anim = isHighlighted ?
                highlightOnAnim :
                highlightOffAnim;
            await button.animator.PlayAnimation(anim, linkedCts.Token);
        }
    }
}
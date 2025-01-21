using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.Animation;
using Noneb.UI.Element;
using NonebNi.Core.Actions;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    public class UnitActionPanel : MonoBehaviour, IElementComponent
    {
        [SerializeField] private GameObject actionsRoot = null!;
        [SerializeField] private ActionOption actionOptionPrefab = null!;

        [SerializeField] private Animator panelAnimator = null!;
        [SerializeField] private AnimationData inspectionMode = null!;
        [SerializeField] private AnimationData controlMode = null!;


        private NonebAction? _highlightedAction;
        private bool _isInspectionMode;

        public event Action<NonebAction?>? ActionSelected;

        public async UniTask Show(NonebAction[] actions, bool isInspectionMode, CancellationToken ct = default)
        {
            var tasks = new List<UniTask>();

            foreach (var actionButton in actionsRoot.GetComponentsInChildren<ActionOption>()) Destroy(actionButton.gameObject);

            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            foreach (var action in actions)
            {
                var (isSuccess, actionButton) = await NonebElement.CreateElementFromPrefab(actionOptionPrefab, actionsRoot.transform);
                if (!isSuccess)
                {
                    Log.Error("Failed element creation");
                    continue;
                }

                actionButton!.Clicked += OnActionButtonClicked;
                tasks.Add(actionButton.Show(action, linkedCts.Token));
            }

            tasks.Add(SetInspectionMode(isInspectionMode, linkedCts.Token));
            var showTasks = UniTask.WhenAll(tasks);
            await showTasks;
        }

        private async UniTask SetInspectionMode(bool isInspectionMode, CancellationToken ct = default)
        {
            _isInspectionMode = isInspectionMode;
            if (_isInspectionMode)
                await panelAnimator.PlayAnimation(inspectionMode, ct);
            else
                await panelAnimator.PlayAnimation(controlMode, ct);
        }

        private void OnActionButtonClicked(NonebAction action)
        {
            if (_isInspectionMode) return;

            ActionSelected?.Invoke(action);
        }

        public async UniTask Highlight(NonebAction? action)
        {
            _highlightedAction = action;
            await RefreshHighlight();
        }

        private async UniTask RefreshHighlight()
        {
            var tasks = new List<UniTask>();
            foreach (var actionButton in actionsRoot.GetComponentsInChildren<ActionOption>())
            {
                var isHighlighted = actionButton.Action == _highlightedAction;
                var task = actionButton.SetHighlight(isHighlighted);
                tasks.Add(task);
            }

            await UniTask.WhenAll(tasks);
        }
    }
}
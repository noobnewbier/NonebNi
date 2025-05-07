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
        private CancellationTokenSource? _cts;

        private bool _isInspectionMode;

        public NonebAction? SelectedAction { get; private set; }

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

        public void Select(NonebAction? action)
        {
            SelectedAction = action;
            RefreshHighlight();
        }

        private void RefreshHighlight()
        {
            async UniTaskVoid Do(CancellationToken ct)
            {
                var tasks = new List<UniTask>();
                foreach (var actionButton in actionsRoot.GetComponentsInChildren<ActionOption>())
                {
                    var isHighlighted = actionButton.Action == SelectedAction;
                    var task = actionButton.SetHighlight(isHighlighted, ct);
                    tasks.Add(task);
                }

                await UniTask.WhenAll(tasks);
            }

            _cts?.Cancel();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
            Do(_cts.Token).Forget();
        }
    }
}
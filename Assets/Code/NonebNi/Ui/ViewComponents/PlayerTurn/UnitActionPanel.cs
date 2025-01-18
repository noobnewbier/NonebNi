using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
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

        private NonebAction? _highlightedAction;

        public event Action<NonebAction?>? ActionSelected;

        public async UniTask Show(NonebAction[] actions, CancellationToken ct = default)
        {
            foreach (var actionButton in actionsRoot.GetComponentsInChildren<ActionOption>()) Destroy(actionButton.gameObject);

            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var tasks = new List<UniTask>();
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

            var showTasks = UniTask.WhenAll(tasks);
            await showTasks;
        }

        private void OnActionButtonClicked(NonebAction action)
        {
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
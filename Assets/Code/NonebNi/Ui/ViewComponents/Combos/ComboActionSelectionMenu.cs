using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NonebNi.Core.Actions;
using NonebNi.Core.Decisions;
using NonebNi.Core.Units;
using NonebNi.Ui.Cameras;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using UnityEngine;

namespace NonebNi.Ui.ViewComponents.Combos
{
    public interface IComboActionSelectionMenu : IViewComponent<IComboActionSelectionMenu.Data>
    {
        public record Data(UnitData ActiveUnit, IEnumerable<NonebAction> PossibleComboActions);

        public record UIInput(IDecision Decision);
    }

    public class ComboActionSelectionMenu : MonoBehaviour, IComboActionSelectionMenu
    {
        [SerializeField] private UnitActionPanel actionPanel = null!;
        [SerializeField] private UnitDetailsPanel detailsPanel = null!;

        private CancellationTokenSource? _cts;
        private IComboActionSelectionMenu.Data? _data;
        private Dependencies _deps = null!;

        public void Init(Dependencies dependencies)
        {
            _deps = dependencies;
        }

        public UniTask OnViewActivate(IComboActionSelectionMenu.Data? viewData)
        {
            _data = viewData;

            actionPanel.ActionSelected += OnActionSelected;
            return UniTask.CompletedTask;
        }

        public async UniTask OnViewEnter(INonebView? previousView, INonebView currentView)
        {
            if (_data == null) return;

            await ShowUnit(_data.ActiveUnit, true);
        }

        public UniTask OnViewDeactivate()
        {
            actionPanel.ActionSelected -= OnActionSelected;

            return UniTask.CompletedTask;
        }

        private void OnActionSelected(NonebAction? action)
        {
            SelectAction(action);
        }

        private async UniTask ShowUnit(UnitData unit, bool isUnitActive, CancellationToken ct = default)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);

            _deps.CameraController.LookAt(unit);
            detailsPanel.Show(unit);
            await actionPanel.Show(unit.Actions, !isUnitActive, linkedCts.Token);
            SelectAction(null);
        }

        private void SelectAction(NonebAction? action)
        {
            async UniTaskVoid Do(CancellationToken ct)
            {
                actionPanel.Select(action);
                await _deps.ActionInputControl.SetActionContext(detailsPanel.ShownUnit, actionPanel.SelectedAction, true, ct);
            }

            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            Do(_cts.Token).Forget();
        }

        public record Dependencies(ICameraController CameraController, IActionInputControl ActionInputControl);
    }
}
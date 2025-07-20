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
        //todo: possible combo - use them
        //todo: reenter same unit should not leave view
        //todo: when comboing https://privatebin.net/?11090be6f34f7d9d#ARRBfCgcANGSsTnrtNod9YpeLzyhNKtMNeRbVkLTEqxH
        //todo: when combo can infinite
        //todo: path find -> walk around enemy
        //todo: need to figure out the awkwardness when the game start...
        //todo: input get cancelled

        public record Data(UnitData ActiveUnit, IEnumerable<NonebAction> PossibleComboActions, UIInputReader<UIInput> InputReader);

        public record UIInput(IDecision Decision);
    }

    public class ComboActionSelectionMenu : MonoBehaviour, IComboActionSelectionMenu
    {
        [SerializeField] private UnitActionPanel actionPanel = null!;
        [SerializeField] private UnitDetailsPanel detailsPanel = null!;

        private CancellationTokenSource _cts = new();
        private IComboActionSelectionMenu.Data? _data;
        private Dependencies _deps = null!;

        public void Init(Dependencies dependencies)
        {
            _deps = dependencies;
        }

        public UniTask OnViewActivate(IComboActionSelectionMenu.Data? viewData)
        {
            _data = viewData;
            _cts = new CancellationTokenSource();

            actionPanel.ActionSelected += OnActionSelected;
            return UniTask.CompletedTask;
        }

        public async UniTask OnViewEnter(INonebView? previousView, INonebView currentView)
        {
            if (_data == null) return;

            await ShowUnit(_data.ActiveUnit, true);
            WaitForUserInput(_cts.Token).Forget();
        }

        public UniTask OnViewDeactivate()
        {
            _cts.Cancel();
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
            actionPanel.Select(action);
            _deps.ActionDecisionFlowControl.UpdateActionContext(detailsPanel.ShownUnit, actionPanel.SelectedAction, true);
        }

        private async UniTaskVoid WaitForUserInput(CancellationToken ct)
        {
            var decision = await _deps.ActionDecisionFlowControl.WaitForUserInput(ct);
            ct.ThrowIfCancellationRequested();

            _data?.InputReader.Write(new IComboActionSelectionMenu.UIInput(decision));
        }

        public record Dependencies(ICameraController CameraController, IActionDecisionFlowControl ActionDecisionFlowControl);
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NonebNi.Core.Actions;
using NonebNi.Core.Units;
using NonebNi.Ui.Cameras;
using UnityEngine;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    //TODO: root handle
    //TODO: someone to init/inject dependencies
    //TODO: clear up folder structure -> make assets feature based as it's confusing the crap out of me, code can stay where it is though. 
    public interface IPlayerTurnMenu
    {
        public event Action<NonebAction?>? ActionSelected;
        public event Action<UnitData?>? UnitSelected;
        UniTask SelectAction(NonebAction? action);
        UniTask ShowUnit(UnitData unit, bool isUnitActive, CancellationToken ct = default);
        UniTask ShowActOrder(IEnumerable<UnitData> unitsInOrder, CancellationToken ct = default);
    }

    public class PlayerTurnMenu : MonoBehaviour, IViewComponent, IPlayerTurnMenu
    {
        [SerializeField] private GameObject subStackRoot = null!;
        [SerializeField] private UnitActionPanel actionPanel = null!;
        [SerializeField] private UnitActOrderPanel orderPanel = null!;
        [SerializeField] private UnitDetailsPanel detailsPanel = null!;
        private ICameraController _cameraController = null!;
        private CancellationTokenSource? _executeActionFlowCts;

        private IPlayerTurnPresenter _presenter = null!;
        private UIStack _stack = null!;
        private IPlayerTurnWorldSpaceInputControl _worldSpaceInputControl = null!;

        public event Action<NonebAction?>? ActionSelected;
        public event Action<UnitData?>? UnitSelected;

        public async UniTask SelectAction(NonebAction? action)
        {
            RefreshControlMode();
            await actionPanel.Highlight(action);
        }

        public async UniTask ShowUnit(UnitData unit, bool isUnitActive, CancellationToken ct = default)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);

            var targetUnitPos = _presenter.FindUnitPosition(unit);
            _cameraController.LookAt(targetUnitPos);
            await UniTask.WhenAll(
                actionPanel.Show(unit.Actions, !isUnitActive, linkedCts.Token),
                detailsPanel.Show(unit, linkedCts.Token)
            );
        }

        public async UniTask ShowActOrder(IEnumerable<UnitData> unitsInOrder, CancellationToken ct = default)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);
            await orderPanel.Show(unitsInOrder, linkedCts.Token);
        }

        public async UniTask OnViewEnter(INonebView? previousView)
        {
            await _presenter.EnterView();
            actionPanel.ActionSelected += ActionSelected;
            orderPanel.UnitSelected += UnitSelected;
        }

        public UniTask OnViewLeave(INonebView? nextView)
        {
            actionPanel.ActionSelected -= ActionSelected;
            orderPanel.UnitSelected -= UnitSelected;

            return UniTask.CompletedTask;
        }

        private void RefreshControlMode()
        {
            _executeActionFlowCts?.Cancel();
            if (_presenter.SelectedAction == null)
                //TODO: probs more complicated 
                _worldSpaceInputControl.ToTileInspectionMode();
            else
            {
                _executeActionFlowCts = new CancellationTokenSource();
                ExecuteActionFlow(_executeActionFlowCts.Token).Forget();
            }
        }

        private async UniTask ExecuteActionFlow(CancellationToken ct = default)
        {
            if (_presenter.SelectedAction == null) return;

            var input = await _worldSpaceInputControl.GetInputForAction(_presenter.InspectingUnit, _presenter.SelectedAction, ct);
            _presenter.MakeDecision(input);
            //todo: do something with that input mate.
        }

        //TODO: work out the inject process with strong ioc.
        public void Inject(IPlayerTurnPresenter presenter, IPlayerTurnWorldSpaceInputControl worldSpaceInputControl, ICameraController cameraController)
        {
            _presenter = presenter;
            _stack = new UIStack(subStackRoot);
            _worldSpaceInputControl = worldSpaceInputControl;
            _cameraController = cameraController;
        }
    }
}
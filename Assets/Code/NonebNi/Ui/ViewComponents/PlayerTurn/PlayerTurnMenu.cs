using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NonebNi.Core.Actions;
using NonebNi.Core.Agents;
using NonebNi.Core.Decisions;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Units;
using NonebNi.Ui.Cameras;
using UnityEngine;
using UnityEngine.UI;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    //todo: making player turn works. focus on that.
    //todo: next step would be sorting out movement
    //todo: and then we would need a "rest/end turn action" for doing nothing.

    //TODO: root handle
    //TODO: someone to init/inject dependencies
    //TODO: clear up folder structure -> make assets feature based as it's confusing the crap out of me, code can stay where it is though. 
    public interface IPlayerTurnMenu : IViewComponent<IPlayerTurnMenu.Data>
    {
        public record Data(UnitData ActiveUnit);
    }

    public class PlayerTurnMenu : MonoBehaviour, IPlayerTurnMenu
    {
        [SerializeField] private GameObject subStackRoot = null!;
        [SerializeField] private UnitActionPanel actionPanel = null!;
        [SerializeField] private UnitActOrderPanel orderPanel = null!;
        [SerializeField] private UnitDetailsPanel detailsPanel = null!;

        //TODO: at some point this might go somewhere but I am not too fuzzed about a testing UI
        [SerializeField] private Button endTurnButton = null!;
        private IActionInputControl _actionInputControl = null!;

        private IPlayerAgent _agent = null!;
        private ICameraController _cameraController = null!;
        private CancellationTokenSource? _controlModeCts;

        private IPlayerTurnMenu.Data? _data;
        private IUnitTurnOrderer _unitTurnOrderer = null!;

        public void Init(Dependencies dependencies)
        {
            _actionInputControl = dependencies.ActionInputControl;
            _cameraController = dependencies.CameraController;
            _agent = dependencies.Agent;
            _unitTurnOrderer = dependencies.UnitTurnOrderer;

            endTurnButton.onClick.AddListener(EndTurn);
        }

        public UniTask OnViewActivate(IPlayerTurnMenu.Data? viewData)
        {
            _data = viewData;

            actionPanel.ActionSelected += OnActionSelected;
            orderPanel.UnitSelected += OnUnitSelected;
            return UniTask.CompletedTask;
        }

        public async UniTask OnViewEnter(INonebView? previousView, INonebView currentView)
        {
            if (_data == null) return;

            //todo: current unit timing issue -> we need to pass in data as the push pop happens
            await UniTask.WhenAll(
                ShowUnit(_data.ActiveUnit, true),
                ShowActOrder(_unitTurnOrderer.GetActOrderForTurns(10))
            );
        }

        public UniTask OnViewDeactivate()
        {
            _controlModeCts?.Cancel();

            actionPanel.ActionSelected -= OnActionSelected;
            orderPanel.UnitSelected -= OnUnitSelected;

            return UniTask.CompletedTask;
        }

        private void SelectAction(NonebAction? action)
        {
            actionPanel.Select(action);
            RefreshControlMode();
        }

        private async UniTask ShowUnit(UnitData unit, bool isUnitActive, CancellationToken ct = default)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);

            _cameraController.LookAt(unit);
            detailsPanel.Show(unit);
            await actionPanel.Show(unit.Actions, !isUnitActive, linkedCts.Token);
            SelectAction(null);
        }

        private async UniTask ShowActOrder(IEnumerable<UnitData> unitsInOrder, CancellationToken ct = default)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);
            await orderPanel.Show(unitsInOrder, linkedCts.Token);
        }

        private void RefreshControlMode()
        {
            async UniTaskVoid Do(CancellationToken ct)
            {
                var unitContext = detailsPanel.ShownUnit;
                var isActiveUnit = unitContext == _data?.ActiveUnit;
                var actionContext = actionPanel.SelectedAction;
                if (actionContext == null)
                    if (isActiveUnit && unitContext?.Speed > 0)
                        actionContext = ActionDatas.Move;

                await _actionInputControl.SetActionContext(unitContext, actionContext, isActiveUnit, ct);
            }

            _controlModeCts?.Cancel();
            _controlModeCts = new CancellationTokenSource();
            Do(_controlModeCts.Token).Forget();
        }

        private void EndTurn()
        {
            _controlModeCts?.Cancel();
            //todo: at some point we need noneb button, which prevent spam click from breaking the UI, I can't be asked to deal with it every single time.
            _agent.SetDecision(EndTurnDecision.Instance);
        }

        private void OnActionSelected(NonebAction? action)
        {
            SelectAction(action);
        }

        private void OnUnitSelected(UnitData unit)
        {
            async UniTaskVoid Do()
            {
                var isActiveUnit = detailsPanel.ShownUnit == _data?.ActiveUnit;
                await ShowUnit(unit, isActiveUnit);
            }

            Do().Forget();
        }

        public record Dependencies(IActionInputControl ActionInputControl, ICameraController CameraController, IPlayerAgent Agent, IUnitTurnOrderer UnitTurnOrderer);
    }
}
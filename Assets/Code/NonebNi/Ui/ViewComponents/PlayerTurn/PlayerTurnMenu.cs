using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NonebNi.Core.Actions;
using NonebNi.Core.Agents;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Decisions;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Maps;
using NonebNi.Core.Units;
using NonebNi.Terrain;
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

        public NonebAction? SelectedAction { get; private set; }

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
            actionPanel.ActionSelected -= OnActionSelected;
            orderPanel.UnitSelected -= OnUnitSelected;

            return UniTask.CompletedTask;
        }

        private async UniTask SelectAction(NonebAction? action)
        {
            SelectedAction = action;
            RefreshControlMode();
            await actionPanel.Highlight(action);
        }

        private async UniTask ShowUnit(UnitData unit, bool isUnitActive, CancellationToken ct = default)
        {
            _selectedUnit = unit;

            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);

            var targetUnitPos = FindUnitPosition(unit);
            _cameraController.LookAt(targetUnitPos);
            detailsPanel.Show(unit);
            await actionPanel.Show(unit.Actions, !isUnitActive, linkedCts.Token);
            await SelectAction(null);
        }

        private async UniTask ShowActOrder(IEnumerable<UnitData> unitsInOrder, CancellationToken ct = default)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);
            await orderPanel.Show(unitsInOrder, linkedCts.Token);
        }

        //todo: something is wrong with the inspected unit, is it with DI doing weird stuffs?
        private void RefreshControlMode()
        {
            _controlModeCts?.Cancel();
            _controlModeCts = new CancellationTokenSource();

            if (_selectedUnit == null)
                // We ain't controlling nothing
                return;

            if (SelectedAction == null)
            {
                if (_selectedUnit == _data?.ActiveUnit && _selectedUnit.Speed > 0)
                    MovementFlow(_controlModeCts.Token).Forget();
                else
                    InspectTileFlow(_controlModeCts.Token).Forget();
            }
            else
            {
                ExecuteActionFlow(_controlModeCts.Token).Forget();
            }
        }

        private async UniTask MovementFlow(CancellationToken ct = default)
        {
            if (_selectedUnit == null) return;

            var input = await _worldSpaceInputControl.GetInputForAction(_selectedUnit, ActionDatas.Move, ct);
            ct.ThrowIfCancellationRequested();

            MakeActionDecision(ActionDatas.Move, input);
        }

        private async UniTask InspectTileFlow(CancellationToken ct = default) =>
            //todo: tile click -> inspect
            _worldSpaceInputControl.ToTileInspectionMode();

        private async UniTask ExecuteActionFlow(CancellationToken ct = default)
        {
            if (SelectedAction == null) return;
            if (_selectedUnit == null) return;

            var input = await _worldSpaceInputControl.GetInputForAction(_selectedUnit, SelectedAction, ct);
            ct.ThrowIfCancellationRequested();

            MakeActionDecision(SelectedAction, input);
        }

        private void MakeActionDecision(NonebAction action, IEnumerable<Coordinate> coordinates)
        {
            if (_selectedUnit == null) return;

            var decision = new ActionDecision(action, _selectedUnit, coordinates);
            _agent.SetDecision(decision);
            //todo: need to transition into another state menu but we can do that later.
        }

        private void EndTurn()
        {
            //todo: at some point we need noneb button, which prevent spam click from breaking the UI, I can't be asked to deal with it every single time.
            _agent.SetDecision(EndTurnDecision.Instance);
        }

        private Vector3 FindUnitPosition(UnitData unit)
        {
            if (!_map.TryFind(unit, out Coordinate coord)) return default;

            var pos = _coordinateAndPositionService.FindPosition(coord);
            return pos;
        }

        private void OnActionSelected(NonebAction? action)
        {
            async UniTaskVoid Do()
            {
                await SelectAction(action);
            }

            Do().Forget();
        }

        private void OnUnitSelected(UnitData unit)
        {
            async UniTaskVoid Do()
            {
                var isActiveUnit = _selectedUnit == _data?.ActiveUnit;
                await ShowUnit(unit, isActiveUnit);
            }

            Do().Forget();
        }

        public record Dependencies(IActionInputControl ActionInputControl, ICameraController CameraController, IPlayerAgent Agent, IUnitTurnOrderer UnitTurnOrderer);
    }
}
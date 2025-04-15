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
using Unity.Logging;
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
    public interface IPlayerTurnMenu : IViewComponent { }

    public class PlayerTurnMenu : MonoBehaviour, IPlayerTurnMenu
    {
        [SerializeField] private GameObject subStackRoot = null!;
        [SerializeField] private UnitActionPanel actionPanel = null!;
        [SerializeField] private UnitActOrderPanel orderPanel = null!;
        [SerializeField] private UnitDetailsPanel detailsPanel = null!;

        //TODO: at some point this might go somewhere but I am not too fuzzed about a testing UI
        [SerializeField] private Button endTurnButton = null!;

        private IPlayerAgent _agent = null!;
        private ICameraController _cameraController = null!;
        private CancellationTokenSource? _controlModeCts;
        private ICoordinateAndPositionService _coordinateAndPositionService = null!;
        private IReadOnlyMap _map = null!;
        private UnitData? _selectedUnit;
        private IUnitTurnOrderer _unitTurnOrderer = null!;
        private IPlayerTurnWorldSpaceInputControl _worldSpaceInputControl = null!;

        public void Init(
            IPlayerTurnWorldSpaceInputControl worldSpaceInputControl,
            ICameraController cameraController,
            IPlayerAgent agent,
            ICoordinateAndPositionService coordinateAndPositionService,
            IReadOnlyMap map,
            IUnitTurnOrderer unitTurnOrderer)
        {
            _worldSpaceInputControl = worldSpaceInputControl;
            _cameraController = cameraController;
            _agent = agent;
            _coordinateAndPositionService = coordinateAndPositionService;
            _map = map;
            _unitTurnOrderer = unitTurnOrderer;

            endTurnButton.onClick.AddListener(EndTurn);
        }

        public UnitData InspectingUnit
        {
            get
            {
                if (_selectedUnit != null) return _selectedUnit;

                return _unitTurnOrderer.CurrentUnit;
            }
        }

        public NonebAction? SelectedAction { get; private set; }

        public async UniTask OnViewEnter(INonebView? previousView)
        {
            actionPanel.ActionSelected += OnActionSelected;
            orderPanel.UnitSelected += OnUnitSelected;

            await ShowCurrentTurnUnit();
        }

        public UniTask OnViewLeave(INonebView? nextView)
        {
            actionPanel.ActionSelected -= OnActionSelected;

            return UniTask.CompletedTask;
        }

        public async UniTask SelectAction(NonebAction? action)
        {
            SelectedAction = action;
            RefreshControlMode();
            await actionPanel.Highlight(action);
        }

        public async UniTask ShowUnit(UnitData unit, bool isUnitActive, CancellationToken ct = default)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);

            var targetUnitPos = FindUnitPosition(unit);
            _cameraController.LookAt(targetUnitPos);
            await UniTask.WhenAll(
                actionPanel.Show(unit.Actions, !isUnitActive, linkedCts.Token),
                detailsPanel.Show(unit, linkedCts.Token)
            );
            await SelectAction(null);
        }

        public async UniTask ShowActOrder(IEnumerable<UnitData> unitsInOrder, CancellationToken ct = default)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);
            await orderPanel.Show(unitsInOrder, linkedCts.Token);
        }

        private void RefreshControlMode()
        {
            _controlModeCts?.Cancel();
            _controlModeCts = new CancellationTokenSource();
            if (SelectedAction == null)
            {
                if (InspectingUnit.Speed > 0)
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
            var input = await _worldSpaceInputControl.GetInputForAction(InspectingUnit, ActionDatas.Move, ct);
            ct.ThrowIfCancellationRequested();

            MakeActionDecision(ActionDatas.Move, input);
        }

        private async UniTask InspectTileFlow(CancellationToken ct = default) =>
            //todo: tile click -> inspect
            _worldSpaceInputControl.ToTileInspectionMode();

        private async UniTask ExecuteActionFlow(CancellationToken ct = default)
        {
            if (SelectedAction == null) return;

            var input = await _worldSpaceInputControl.GetInputForAction(InspectingUnit, SelectedAction, ct);
            ct.ThrowIfCancellationRequested();

            MakeActionDecision(SelectedAction, input);
        }

        public async UniTask ShowCurrentTurnUnit()
        {
            await UniTask.WhenAll(
                ShowUnit(_unitTurnOrderer.CurrentUnit, true),
                ShowActOrder(_unitTurnOrderer.UnitsInOrder)
            );
        }

        private void MakeActionDecision(NonebAction action, IEnumerable<Coordinate> coordinates)
        {
            var decision = new ActionDecision(action, InspectingUnit, coordinates);
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

        private void OnUnitSelected(UnitData? unit)
        {
            async UniTaskVoid Do()
            {
                if (unit == InspectingUnit) return;

                //todo: make this more concise.
                _selectedUnit = unit;

                var isActiveUnit = _unitTurnOrderer.CurrentUnit == InspectingUnit;
                await UniTask.WhenAll(
                    ShowUnit(InspectingUnit, isActiveUnit),
                    SelectAction(null)
                );
            }

            Do().Forget();
        }
    }
}
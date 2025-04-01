using System;
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
    public interface IPlayerTurnMenu { }

    public class PlayerTurnMenu : MonoBehaviour, IViewComponent, IPlayerTurnMenu
    {
        [SerializeField] private GameObject subStackRoot = null!;
        [SerializeField] private UnitActionPanel actionPanel = null!;
        [SerializeField] private UnitActOrderPanel orderPanel = null!;
        [SerializeField] private UnitDetailsPanel detailsPanel = null!;

        //TODO: at some point this might go somewhere but I am not too fuzzed about a testing UI
        [SerializeField] private Button endTurnButton = null!;

        private IPlayerAgent _agent = null!;
        private ICameraController _cameraController = null!;
        private ICoordinateAndPositionService _coordinateAndPositionService = null!;
        private CancellationTokenSource? _executeActionFlowCts;
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

            await UniTask.WhenAll(
                SelectAction(null),
                ShowCurrentTurnUnit()
            );
        }

        public UniTask OnViewLeave(INonebView? nextView)
        {
            actionPanel.ActionSelected -= OnActionSelected;
            orderPanel.UnitSelected -= UnitSelected;

            return UniTask.CompletedTask;
        }

        public event Action<NonebAction?>? ActionSelected;
        public event Action<UnitData?>? UnitSelected;

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
        }

        public async UniTask ShowActOrder(IEnumerable<UnitData> unitsInOrder, CancellationToken ct = default)
        {
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, destroyCancellationToken);
            await orderPanel.Show(unitsInOrder, linkedCts.Token);
        }

        private void RefreshControlMode()
        {
            _executeActionFlowCts?.Cancel();
            if (SelectedAction == null)
            {
                if (InspectingUnit.Speed > 0)
                    _worldSpaceInputControl.ToMovementMode(InspectingUnit);
                else
                    _worldSpaceInputControl.ToTileInspectionMode();
            }
            else
            {
                _executeActionFlowCts = new CancellationTokenSource();
                ExecuteActionFlow(_executeActionFlowCts.Token).Forget();
            }
        }

        private async UniTask ExecuteActionFlow(CancellationToken ct = default)
        {
            if (SelectedAction == null) return;

            var input = await _worldSpaceInputControl.GetInputForAction(InspectingUnit, SelectedAction, ct);
            MakeActionDecision(input);
            //todo: do something with that input mate.
        }

        //TODO: work out the inject process with strong ioc.
        public void RefreshForNewTurn()
        {
            throw new NotImplementedException();
        }

        private async UniTask ShowCurrentTurnUnit()
        {
            await UniTask.WhenAll(
                ShowUnit(_unitTurnOrderer.CurrentUnit, true),
                ShowActOrder(_unitTurnOrderer.UnitsInOrder)
            );
        }

        private void MakeActionDecision(IEnumerable<Coordinate> coordinates)
        {
            if (SelectedAction == null)
            {
                Log.Error("Selected action is null - how did you even get here?");
                return;
            }

            var decision = new ActionDecision(SelectedAction, InspectingUnit, coordinates);
            _agent.SetDecision(decision);
            //todo: need to transition into another state menu but we can do that later.
        }

        private void EndTurn()
        {
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
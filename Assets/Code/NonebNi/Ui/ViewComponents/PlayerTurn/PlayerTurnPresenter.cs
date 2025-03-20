using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Actions;
using NonebNi.Core.Agents;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Decisions;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Maps;
using NonebNi.Core.Units;
using NonebNi.Terrain;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    public interface IPlayerTurnPresenter
    {
        UnitData InspectingUnit { get; }
        NonebAction? SelectedAction { get; }
        UniTask EnterView();
        Vector3 FindUnitPosition(UnitData unit);
        void MakeActionDecision(IEnumerable<Coordinate> coordinates);
        void EndTurn();
    }

    public class PlayerTurnPresenter : IPlayerTurnPresenter
    {
        private readonly IPlayerAgent _agent;
        private readonly ICoordinateAndPositionService _coordinateAndPositionService;
        private readonly IReadOnlyMap _map;
        private readonly IUnitTurnOrderer _unitTurnOrderer;
        private readonly IPlayerTurnMenu _view;

        private UnitData? _selectedUnit;

        public PlayerTurnPresenter(IPlayerTurnMenu view, IUnitTurnOrderer unitTurnOrderer, ICoordinateAndPositionService coordinateAndPositionService, IReadOnlyMap map, IPlayerAgent agent)
        {
            _view = view;
            _unitTurnOrderer = unitTurnOrderer;
            _coordinateAndPositionService = coordinateAndPositionService;
            _map = map;
            _agent = agent;

            view.UnitSelected += unit => UniTask.Action(unit, OnUnitSelected).Invoke();
            view.ActionSelected += action => UniTask.Action(action, OnActionSelected).Invoke();
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

        //TODO: work out if/should/ enterview or other uistack op have ct handling. It probably should(parent could get destroyed)...
        public async UniTask EnterView()
        {
            await SelectAction(null);

            await UniTask.WhenAll(
                _view.ShowUnit(_unitTurnOrderer.CurrentUnit, true),
                _view.ShowActOrder(_unitTurnOrderer.UnitsInOrder)
            );
        }

        public void MakeActionDecision(IEnumerable<Coordinate> coordinates)
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

        public void EndTurn()
        {
            _agent.SetDecision(EndTurnDecision.Instance);
        }

        public Vector3 FindUnitPosition(UnitData unit)
        {
            if (!_map.TryFind(unit, out Coordinate coord)) return default;

            var pos = _coordinateAndPositionService.FindPosition(coord);
            return pos;
        }

        private async UniTask SelectAction(NonebAction? action)
        {
            SelectedAction = action;
            await _view.SelectAction(action);
        }

        private async UniTaskVoid OnActionSelected(NonebAction? action)
        {
            await SelectAction(action);
        }

        private async UniTaskVoid OnUnitSelected(UnitData? unit)
        {
            if (unit == InspectingUnit) return;

            //todo: make this more concise.
            _selectedUnit = unit;

            var isActiveUnit = _unitTurnOrderer.CurrentUnit == InspectingUnit;
            await UniTask.WhenAll(
                _view.ShowUnit(InspectingUnit, isActiveUnit),
                SelectAction(null)
            );
        }
    }
}
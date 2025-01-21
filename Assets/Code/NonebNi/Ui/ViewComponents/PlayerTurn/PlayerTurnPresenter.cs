using Cysharp.Threading.Tasks;
using NonebNi.Core.Actions;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Units;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    public interface IPlayerTurnPresenter
    {
        UnitData InspectingUnit { get; }
        NonebAction? SelectedAction { get; }
        UniTask EnterView();
    }

    public class PlayerTurnPresenter : IPlayerTurnPresenter
    {
        private readonly IUnitTurnOrderer _unitTurnOrderer;
        private readonly IPlayerTurnMenu _view;

        private UnitData? _selectedUnit;

        public PlayerTurnPresenter(IPlayerTurnMenu view, IUnitTurnOrderer unitTurnOrderer)
        {
            _view = view;
            _unitTurnOrderer = unitTurnOrderer;

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
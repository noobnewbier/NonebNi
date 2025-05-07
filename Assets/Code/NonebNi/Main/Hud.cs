using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NonebNi.Core.Agents;
using NonebNi.Core.Commands;
using NonebNi.Core.Decisions;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Units;
using NonebNi.Ui.Attributes;
using NonebNi.Ui.ViewComponents.Combos;
using NonebNi.Ui.ViewComponents.EnemyTurnMenu;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using UnityEngine;

namespace NonebNi.Main
{
    [NonebUniversalEditor]
    public class Hud : MonoBehaviour
    {
        //todo: perhaps these should be injected from level UI, or would the UI be too hard to debug from engine?
        [SerializeField] private PlayerTurnMenu playerTurnMenu = null!;
        [SerializeField] private EnemyTurnMenu enemyTurnMenu = null!;
        [SerializeField] private ComboActionSelectionMenu comboActionSelectionMenu = null!;
        [SerializeField] private ComboUnitSelectionMenu comboUnitSelectionMenu = null!;

        private Dependencies _deps = null!;
        private UIStack _stack = null!;

        public void Init(Dependencies dependencies)
        {
            _deps = dependencies;
            _stack = new UIStack(gameObject);

            playerTurnMenu.Init(_deps.PlayerTurnMenuDeps);
            comboActionSelectionMenu.Init(_deps.ComboActionSelectionMenuDeps);
            comboUnitSelectionMenu.Init(_deps.ComboUnitSelectionMenuDeps);

            ShowActiveUnitControlMenu(_deps.UnitTurnOrderer.CurrentUnit);
        }

        public void ShowActiveUnitControlMenu(UnitData currentUnit)
        {
            //todo: cts support?
            async UniTask Do()
            {
                if (currentUnit.FactionId == _deps.Agent.Faction.Id)
                    await _stack.ReplaceCurrent(playerTurnMenu, new IPlayerTurnMenu.Data(currentUnit));
                else
                    await _stack.ReplaceCurrent(enemyTurnMenu);
            }

            Do().Forget();
        }

        public void ShowComboUI(UnitData unit, IEnumerable<ICommand> possibleCombos)
        {
            async UniTask Do()
            {
                possibleCombos = possibleCombos as ICommand[] ?? possibleCombos.ToArray();
                var actionCommands = possibleCombos.OfType<ActionCommand>().ToArray();
                var possibleUnit = actionCommands
                    .Select(c => c.ActorEntity)
                    .OfType<UnitData>()
                    .ToArray();

                //todo: we successfully freezed the game by combo oh ya
                IDecision? userDecision = null;
                while (userDecision == null)
                {
                    // pick a unit
                    var unitInput = await WaitForUnitSelection(possibleUnit);
                    if (unitInput.Unit == null)
                    {
                        // user wants to pass -> just go away, live and let live you know
                        userDecision = EndTurnDecision.Instance;
                        continue;
                    }

                    // decide which action/who to target
                    userDecision = await WaitComboActionSelection(unitInput.Unit, actionCommands);
                    if (userDecision == null)
                        // user is wishy washy, go back to unit selection
                        await _stack.Pop();
                }

                _deps.Agent.SetDecision(EndTurnDecision.Instance);
            }

            Do().Forget();
            return;

            async UniTask<IDecision?> WaitComboActionSelection(UnitData selectedUnit, ActionCommand[] availableCombos)
            {
                var reader = new UIInputReader<IComboActionSelectionMenu.UIInput>();
                var possibleActionForSelectedUnit = availableCombos
                    .Where(c => c.ActorEntity == selectedUnit)
                    .Select(a => a.Action);
                await _stack.ReplaceCurrent(comboActionSelectionMenu, new IComboActionSelectionMenu.Data(unit, possibleActionForSelectedUnit));
                var uiInput = await reader.Read();

                return uiInput.Decision;
            }

            async UniTask<IComboUnitSelectionMenu.UIInput> WaitForUnitSelection(UnitData[] possibleUnit)
            {
                var reader = new UIInputReader<IComboUnitSelectionMenu.UIInput>();
                await _stack.Push(comboUnitSelectionMenu, new IComboUnitSelectionMenu.Data(possibleUnit, reader));

                var uiInput = await reader.Read();
                return uiInput;
            }
        }

        public record Dependencies(
            PlayerTurnMenu.Dependencies PlayerTurnMenuDeps,
            ComboActionSelectionMenu.Dependencies ComboActionSelectionMenuDeps,
            ComboUnitSelectionMenu.Dependencies ComboUnitSelectionMenuDeps,
            IPlayerAgent Agent,
            IUnitTurnOrderer UnitTurnOrderer);
    }
}
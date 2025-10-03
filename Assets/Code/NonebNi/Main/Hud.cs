using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NonebNi.Core.Agents;
using NonebNi.Core.Commands;
using NonebNi.Core.Decisions;
using NonebNi.Core.FlowControl;
using NonebNi.Core.GameContexts;
using NonebNi.Core.Units;
using NonebNi.Ui.Attributes;
using NonebNi.Ui.ViewComponents.Combos;
using NonebNi.Ui.ViewComponents.EnemyTurnMenu;
using NonebNi.Ui.ViewComponents.HexTooltip;
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

        public void Init(Dependencies dependencies, UIStack stack)
        {
            _deps = dependencies;
            _stack = stack;

            playerTurnMenu.Init(_deps.PlayerTurnMenuDeps);
            comboActionSelectionMenu.Init(_deps.ComboActionSelectionMenuDeps);
            comboUnitSelectionMenu.Init(_deps.ComboUnitSelectionMenuDeps);

            ActiveUnitControlFlow(_deps.UnitTurnOrderer.CurrentUnit);
        }

        public void ActiveUnitControlFlow(UnitData currentUnit)
        {
            //todo: cts support?
            async UniTask Do()
            {
                if (currentUnit.FactionId == _deps.Agent.Faction.Id)
                {
                    var reader = new UIInputReader<IPlayerTurnMenu.UIInput>();
                    await _stack.ReplaceStack(playerTurnMenu, new IPlayerTurnMenu.Data(currentUnit, reader));

                    var input = await reader.Read();
                    _deps.Agent.SetDecision(input.Decision);
                }
                else
                    await _stack.ReplaceCurrent(enemyTurnMenu);
            }

            Do().Forget();
        }

        public void ComboUIFlow(IEnumerable<ICommand> possibleCombos)
        {
            async UniTask Do()
            {
                possibleCombos = possibleCombos as ICommand[] ?? possibleCombos.ToArray();
                var actionCommands = possibleCombos.OfType<ActionCommand>().ToArray();
                var possibleUnit = actionCommands
                    .Select(c => c.ActorEntity)
                    .OfType<UnitData>()
                    .ToArray();

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

                _deps.Agent.SetDecision(userDecision);
            }

            Do().Forget();
            return;

            async UniTask<IDecision?> WaitComboActionSelection(UnitData selectedUnit, ActionCommand[] availableCombos)
            {
                var reader = new UIInputReader<IComboActionSelectionMenu.UIInput>();
                var possibleActionForSelectedUnit = availableCombos
                    .Where(c => c.ActorEntity == selectedUnit)
                    .Select(a => a.Action);
                await _stack.Push(comboActionSelectionMenu, new IComboActionSelectionMenu.Data(selectedUnit, possibleActionForSelectedUnit, reader));
                var uiInput = await reader.Read();

                return uiInput.Decision;
            }

            async UniTask<IComboUnitSelectionMenu.UIInput> WaitForUnitSelection(UnitData[] possibleUnit)
            {
                /*
                 * We start the reading process first, this prevents an issue where if user clicks too fast before we start reading and after the view is pushed
                 * in which case the user is writing input before we starts reading, which fucks things up
                 *
                 * We can in theory change the Read() bit so it works better, but for now it's "good enough".
                 */
                var uiInput = comboUnitSelectionMenu.Read();

                if (!_stack.IsCurrentComponent(comboUnitSelectionMenu)) await _stack.Push(comboUnitSelectionMenu, new IComboUnitSelectionMenu.Data(possibleUnit));

                return await uiInput;
            }
        }

        public record Dependencies
        {
            public Dependencies(
                PlayerTurnMenu.Dependencies PlayerTurnMenuDeps,
                ComboActionSelectionMenu.Dependencies ComboActionSelectionMenuDeps,
                ComboUnitSelectionMenu.Dependencies ComboUnitSelectionMenuDeps,
                KeyedInject<DiKeys.PlayerAgent, IWaitForExternalInputAgent> Agent,
                IUnitTurnOrderer UnitTurnOrderer,
                HexTooltipControl.Dependencies HexTooltipControlDeps)
            {
                this.PlayerTurnMenuDeps = PlayerTurnMenuDeps;
                this.ComboActionSelectionMenuDeps = ComboActionSelectionMenuDeps;
                this.ComboUnitSelectionMenuDeps = ComboUnitSelectionMenuDeps;
                this.Agent = Agent.Value;
                this.UnitTurnOrderer = UnitTurnOrderer;
                this.HexTooltipControlDeps = HexTooltipControlDeps;
            }

            public PlayerTurnMenu.Dependencies PlayerTurnMenuDeps { get; init; }
            public ComboActionSelectionMenu.Dependencies ComboActionSelectionMenuDeps { get; init; }
            public ComboUnitSelectionMenu.Dependencies ComboUnitSelectionMenuDeps { get; init; }
            public IWaitForExternalInputAgent Agent { get; init; }
            public IUnitTurnOrderer UnitTurnOrderer { get; init; }
            public HexTooltipControl.Dependencies HexTooltipControlDeps { get; init; }
        }
    }
}
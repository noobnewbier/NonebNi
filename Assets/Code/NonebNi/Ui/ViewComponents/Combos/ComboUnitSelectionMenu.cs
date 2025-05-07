using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NonebNi.Core.Maps;
using NonebNi.Core.Units;
using NonebNi.Ui.Cameras;
using NonebNi.Ui.Grids;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using UnityEngine;
using UnityEngine.UI;

namespace NonebNi.Ui.ViewComponents.Combos
{
    public interface IComboUnitSelectionMenu : IViewComponent<IComboUnitSelectionMenu.Data>
    {
        public record UIInput(UnitData? Unit);

        public record Data(IEnumerable<UnitData> PossibleComboTakers, UIInputReader<UIInput> UIInputReader);
    }

    //todo: wbn if we have a template for this paradigm
    public class ComboUnitSelectionMenu : MonoBehaviour, IComboUnitSelectionMenu
    {
        [SerializeField] private Button passButton = null!;

        private CancellationTokenSource? _cts;

        private IComboUnitSelectionMenu.Data? _data;
        private Dependencies _deps = null!;

        public void Init(Dependencies dependencies)
        {
            _deps = dependencies;

            passButton.onClick.AddListener(PassCombo);
        }

        public UniTask OnViewActivate(IComboUnitSelectionMenu.Data? viewData)
        {
            _data = viewData;

            return UniTask.CompletedTask;
        }

        public UniTask OnViewEnter(INonebView? previousView, INonebView currentView)
        {
            if (_data == null) return UniTask.CompletedTask;

            WaitForPlayerInput(_data.PossibleComboTakers);

            return UniTask.CompletedTask;
        }

        public UniTask OnViewLeave(INonebView currentView, INonebView? nextView)
        {
            _cts?.Cancel();
            _deps.HexHighlighter.RemoveRequest(HighlightRequestId.UnitSelectionHint);
            return UniTask.CompletedTask;
        }

        public UniTask OnViewDeactivate() => UniTask.CompletedTask;

        private void WaitForPlayerInput(IEnumerable<UnitData> comboTakers)
        {
            async UniTaskVoid Do(CancellationToken ct)
            {
                comboTakers = comboTakers as UnitData[] ?? comboTakers.ToArray();
                HighlightUnits(comboTakers);

                UnitData? inputFromInspection = null;
                while (inputFromInspection == null)
                {
                    var userInput = await _deps.InputControl.GetInputForInspection(ct);
                    ct.ThrowIfCancellationRequested();

                    if (userInput is not UnitData unit) continue;
                    if (!comboTakers.Contains(unit)) continue;

                    inputFromInspection = unit;
                }

                ProcessOutput(inputFromInspection);
            }

            _cts?.Cancel();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

            Do(_cts.Token).Forget();
        }

        private void PassCombo()
        {
            _cts?.Cancel();

            ProcessOutput(null);
        }

        private void ProcessOutput(UnitData? unit)
        {
            if (unit != null) _deps.CameraController.LookAt(unit);

            var input = new IComboUnitSelectionMenu.UIInput(unit);
            _data?.UIInputReader.Write(input);
        }

        private void HighlightUnits(IEnumerable<UnitData> comboTakers)
        {
            var coords = comboTakers.Select(u => _deps.Map.Find(u));
            _deps.HexHighlighter.RequestHighlight(coords, HighlightRequestId.UnitSelectionHint, HighlightVariation.AreaHint);
        }

        public record Dependencies(IHexHighlighter HexHighlighter, IReadOnlyMap Map, IPlayerTurnWorldSpaceInputControl InputControl, ICameraController CameraController);
    }
}
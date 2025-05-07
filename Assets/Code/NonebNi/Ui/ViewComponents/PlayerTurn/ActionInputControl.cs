using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Actions;
using NonebNi.Core.Agents;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Decisions;
using NonebNi.Core.Units;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    public interface IActionInputControl
    {
        UniTask SetActionContext(UnitData? unit, NonebAction? action, bool isActiveUnit, CancellationToken ct = default);
    }

    public class ActionInputControl : IActionInputControl
    {
        private readonly IPlayerAgent _agent;
        private readonly IPlayerTurnWorldSpaceInputControl _inputControl;

        private CancellationTokenSource? _cts;

        public ActionInputControl(IPlayerAgent agent, IPlayerTurnWorldSpaceInputControl inputControl)
        {
            _agent = agent;
            _inputControl = inputControl;
        }

        public async UniTask SetActionContext(UnitData? unit, NonebAction? action, bool isActiveUnit, CancellationToken ct = default)
        {
            _cts?.Cancel();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);

            if (action == null || unit == null)
            {
                await InspectTileFlow(_cts.Token);
                return;
            }

            var canOnlyInspect = action == ActionDatas.Move && unit.Speed <= 0;
            canOnlyInspect |= !isActiveUnit;
            if (canOnlyInspect)
            {
                await InspectTileFlow(_cts.Token);
                return;
            }

            await ExecuteActionFlow(unit, action, _cts.Token);
        }

        private async UniTask InspectTileFlow(CancellationToken ct = default) =>
            //todo: tile click -> inspect
            _inputControl.ToTileInspectionMode();

        private async UniTask ExecuteActionFlow(UnitData unit, NonebAction action, CancellationToken ct = default)
        {
            var input = await _inputControl.GetInputForAction(unit, action, ct);
            ct.ThrowIfCancellationRequested();

            MakeActionDecision(unit, action, input);
        }

        private void MakeActionDecision(UnitData unit, NonebAction action, IEnumerable<Coordinate> coordinates)
        {
            var decision = new ActionDecision(action, unit, coordinates);
            _agent.SetDecision(decision);
            //todo: need to transition into another state menu but we can do that later.
        }
    }
}
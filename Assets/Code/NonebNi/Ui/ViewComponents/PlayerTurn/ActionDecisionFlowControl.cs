using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Actions;
using NonebNi.Core.Decisions;
using NonebNi.Core.Units;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    public interface IActionDecisionFlowControl
    {
        UniTask<ActionDecision> WaitForUserInput(CancellationToken ct = default);
        void UpdateActionContext(UnitData? unit, NonebAction? action, bool isActiveUnit);
    }

    //todo: this class seems weird, Idk what but sth is wronng...?
    public class ActionDecisionFlowControl : IActionDecisionFlowControl
    {
        private readonly IPlayerTurnWorldSpaceInputControl _inputControl;

        private CancellationTokenSource? _cts;
        private UniTaskCompletionSource<ActionDecision>? _tcs;

        public ActionDecisionFlowControl(IPlayerTurnWorldSpaceInputControl inputControl)
        {
            _inputControl = inputControl;
        }

        public async UniTask<ActionDecision> WaitForUserInput(CancellationToken ct = default)
        {
            //todo: for now we don't allow multiple caller - so just cancel the chap
            _tcs?.TrySetCanceled();
            _tcs = new UniTaskCompletionSource<ActionDecision>();

            ct.Register(
                () =>
                {
                    _ = _tcs?.TrySetCanceled();
                    _cts?.Cancel();
                }
            );
            var result = await _tcs.Task;
            return result;
        }

        public void UpdateActionContext(UnitData? unit, NonebAction? action, bool isActiveUnit)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            if (action == null || unit == null)
            {
                InspectTileFlow(_cts.Token).Forget();
                return;
            }

            var canOnlyInspect = action == ActionDatas.Move && unit.Speed <= 0;
            canOnlyInspect |= !isActiveUnit;
            if (canOnlyInspect)
            {
                InspectTileFlow(_cts.Token).Forget();
                return;
            }

            ActionInputFlow(unit, action, _cts.Token).Forget();
        }

        private async UniTask InspectTileFlow(CancellationToken ct = default) =>
            //todo: tile click -> inspect
            _inputControl.ToTileInspectionMode();

        private async UniTask ActionInputFlow(UnitData unit, NonebAction action, CancellationToken ct = default)
        {
            var input = await _inputControl.GetInputForAction(unit, action, ct);
            ct.ThrowIfCancellationRequested();

            var decision = new ActionDecision(action, unit, input);
            _tcs?.TrySetResult(decision);
        }
    }
}
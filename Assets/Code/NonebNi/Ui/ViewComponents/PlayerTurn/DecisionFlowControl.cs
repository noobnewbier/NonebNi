using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Actions;
using NonebNi.Core.Decisions;
using NonebNi.Core.Units;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    public interface IDecisionFlowControl
    {
        /// <summary>
        /// And you call this
        /// </summary>
        UniTask<IDecision> WaitForUserInput(CancellationToken ct = default);

        /// <summary>
        /// You call this to change how the user interact with the world, and implicitly what you get from WaitForUserInput
        /// </summary>
        UniTask<bool> UpdateDecisionContext(UnitData? unit, NonebAction? action, bool isActiveUnit);
    }

    //todo: this class seems weird, Idk what but sth is wronng...?
    public class DecisionFlowControl : IDecisionFlowControl
    {
        private readonly IPlayerTurnWorldSpaceInputControl _inputControl;

        private CancellationTokenSource? _cts;
        private UniTaskCompletionSource<IDecision>? _tcs;

        public DecisionFlowControl(IPlayerTurnWorldSpaceInputControl inputControl)
        {
            _inputControl = inputControl;
        }

        public async UniTask<IDecision> WaitForUserInput(CancellationToken ct = default)
        {
            //todo: for now we don't allow multiple caller - so just cancel the chap
            _tcs?.TrySetCanceled();
            _tcs = new UniTaskCompletionSource<IDecision>();

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

        public async UniTask<bool> UpdateDecisionContext(UnitData? unit, NonebAction? action, bool isActiveUnit)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            if (action == null || unit == null) return await InspectTileFlow(_cts.Token);

            var canOnlyInspect = action == ActionDatas.Move && unit.Speed <= 0;
            canOnlyInspect |= !isActiveUnit;
            if (canOnlyInspect) return await InspectTileFlow(_cts.Token);

            if (action == ActionDatas.Move) return await MovementInputFlow(unit, _cts.Token);

            var result = await ActionInputFlow(unit, action, _cts.Token);
            return result;
        }

        private async UniTask<bool> InspectTileFlow(CancellationToken ct = default)
        {
            var input = await _inputControl.GetInputForInspection(ct);
            if (input == null) return false;

            var decision = new InspectDecision(input);
            if (_tcs?.TrySetResult(decision) == false) return false;

            return true;
        }

        private async UniTask<bool> ActionInputFlow(UnitData unit, NonebAction action, CancellationToken ct = default)
        {
            var (success, input) = await _inputControl.GetInputForAction(unit, action, ct);
            ct.ThrowIfCancellationRequested();
            if (!success) return false;

            var decision = new ActionDecision(action, unit, input);
            if (_tcs?.TrySetResult(decision) == false) return false;

            return true;
        }

        private async UniTask<bool> MovementInputFlow(UnitData unit, CancellationToken ct = default)
        {
            var input = await _inputControl.GetInputForMovement(unit, ct);
            ct.ThrowIfCancellationRequested();

            switch (input)
            {
                case IPlayerTurnWorldSpaceInputControl.MovementInput.Cancel:
                    return false;

                case IPlayerTurnWorldSpaceInputControl.MovementInput.Inspect inspect:
                {
                    var decision = new InspectDecision(inspect.EntityData);
                    if (_tcs?.TrySetResult(decision) == false) return false;
                    return true;
                }
                case IPlayerTurnWorldSpaceInputControl.MovementInput.MoveTo moveTo:
                {
                    var decision = new ActionDecision(ActionDatas.Move, unit, moveTo.Coordinate);
                    if (_tcs?.TrySetResult(decision) == false) return false;
                    return true;
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(input));
            }
        }
    }
}
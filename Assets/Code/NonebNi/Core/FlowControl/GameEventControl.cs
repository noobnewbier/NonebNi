using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Effects;
using Unity.Logging;

namespace NonebNi.Core.FlowControl
{
    public interface IGameEventControl : IDisposable
    {
        LevelEvent Current { get; }
        EffectResult ActiveActionResult { get; }
        void WriteEvent(LevelEvent value);
        IUniTaskAsyncEnumerable<LevelEvent> Subscribe(CancellationToken ct);
    }

    /// <summary>
    /// This is slowly ballooning - it started off as a way to keep track of the current event.
    /// Is it a red flag?
    /// </summary>
    public class GameEventControl : IGameEventControl
    {
        /// <summary>
        /// If we need a state machine, we can add one later, atm a simple event queue would suffice
        /// </summary>
        /// <returns></returns>
        private readonly Channel<LevelEvent> _channel = Channel.CreateSingleConsumerUnbounded<LevelEvent>();

        private bool _isSubscribed;

        public void Dispose()
        {
            _channel.Writer.TryComplete();
        }

        public LevelEvent Current { get; private set; } = new LevelEvent.None();

        /// <summary>
        /// "Active" Action is the one that we are still evaluating for combo, sequence effect etc.
        /// </summary>
        public EffectResult ActiveActionResult { get; private set; } = EffectResult.Empty;

        public void WriteEvent(LevelEvent value)
        {
            Current = value;
            UpdateComboSequence();

            if (!_channel.Writer.TryWrite(value)) Log.Error("[Level] You should really, never have got here");
        }

        public IUniTaskAsyncEnumerable<LevelEvent> Subscribe(CancellationToken ct)
        {
            if (_isSubscribed) throw new InvalidOperationException("Already Subscribed - currently this can only handle one consumer");

            _isSubscribed = true;
            return _channel.Reader.ReadAllAsync(ct);
        }

        private void UpdateComboSequence()
        {
            ActiveActionResult = Current switch
            {
                LevelEvent.WaitForActiveUnitDecision => EffectResult.Empty,
                LevelEvent.SequenceOccured { Result: { CanCombo: true } } sequenceOccured => sequenceOccured.Result,
                _ => ActiveActionResult
            };
        }
    }
}
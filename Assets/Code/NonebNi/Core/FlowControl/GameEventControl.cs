using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Logging;

namespace NonebNi.Core.FlowControl
{
    public interface IGameEventControl : IDisposable
    {
        LevelEvent Current { get; }
        bool IsEvaluatingCombo { get; }
        void WriteEvent(LevelEvent value);
        IUniTaskAsyncEnumerable<LevelEvent> Subscribe(CancellationToken ct);
    }

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

        public bool IsEvaluatingCombo
        {
            get
            {
                return Current switch
                {
                    LevelEvent.GameStart or LevelEvent.None or LevelEvent.WaitForActiveUnitDecision => false,

                    // slightly weird that sequence occured is here -> but note how as long as we are mid-sequence evaluation and checking for cost,
                    // we are always evaluating for combo, I can regret later
                    LevelEvent.SequenceOccured or
                        LevelEvent.WaitForComboDecision => true,
                    _ => throw new ArgumentOutOfRangeException(nameof(Current))
                };
            }
        }

        public void WriteEvent(LevelEvent value)
        {
            Current = value;
            if (!_channel.Writer.TryWrite(value)) Log.Error("[Level] You should really, never have got here");
        }

        public IUniTaskAsyncEnumerable<LevelEvent> Subscribe(CancellationToken ct)
        {
            if (_isSubscribed) throw new InvalidOperationException("Already Subscribed - currently this can only handle one consumer");

            _isSubscribed = true;
            return _channel.Reader.ReadAllAsync(ct);
        }
    }
}
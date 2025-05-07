using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Logging;

namespace NonebNi.Core.FlowControl
{
    public interface IGameEventControl : IDisposable
    {
        LevelEvent Current { get; }
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
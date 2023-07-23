using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Decisions;

namespace NonebNi.Core.Agents
{
    public interface IPlayerAgent : IAgent
    {
        void SetDecision(IDecision decision);
    }

    /// <summary>
    ///     An <see cref="IAgent" /> that delegates its decision making to outside(e.g console, UI) input.
    /// </summary>
    public class PlayerAgent : IPlayerAgent
    {
        private IDecision? _decision;

        public PlayerAgent(string factionId)
        {
            FactionId = factionId;
        }

        public string FactionId { get; }

        public async UniTask<IDecision?> GetDecision(CancellationToken ct)
        {
            _decision = null;

            await UniTask.WaitUntil(() => _decision != null, cancellationToken: ct).SuppressCancellationThrow();

            return _decision;
        }

        public void SetDecision(IDecision decision)
        {
            _decision = decision;
        }
    }
}
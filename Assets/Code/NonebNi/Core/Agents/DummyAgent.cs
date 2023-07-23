using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Decisions;

namespace NonebNi.Core.Agents
{
    /// <summary>
    ///     A "Dummy" agent is an agent that always simply ends its turn and does nothing
    /// </summary>
    public class DummyAgent : IAgent
    {
        public DummyAgent(string factionId)
        {
            FactionId = factionId;
        }

        public string FactionId { get; }

        public UniTask<IDecision?> GetDecision(CancellationToken ct) => new UniTask<IDecision?>(new EndTurnDecision());
    }
}
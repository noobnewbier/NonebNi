using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Decision;

namespace NonebNi.Core.Agents
{
    /// <summary>
    /// A "Dummy" agent is an agent that always simply ends its turn and does nothing
    /// </summary>
    public class DummyAgent : IAgent
    {
        public DummyAgent(string factionId)
        {
            FactionId = factionId;
        }

        public string FactionId { get; }

        public UniTask<IDecision?> GetDecision(CancellationToken ct)
        {
            return new UniTask<IDecision?>(new EndTurnDecision());
        }
    }
}
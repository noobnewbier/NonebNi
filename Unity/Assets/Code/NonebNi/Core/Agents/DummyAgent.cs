using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Decisions;
using NonebNi.Core.Factions;

namespace NonebNi.Core.Agents
{
    /// <summary>
    ///     A "Dummy" agent is an agent that always simply ends its turn and does nothing
    /// </summary>
    public class DummyAgent : IAgent
    {
        public DummyAgent(Faction faction)
        {
            Faction = faction;
        }

        public Faction Faction { get; }

        public UniTask<IDecision?> GetDecision(CancellationToken ct) => new(new EndTurnDecision());
    }
}
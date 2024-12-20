using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Decisions;
using NonebNi.Core.Factions;

namespace NonebNi.Core.Agents
{
    /// <summary>
    ///     An "Agent" is an abstract concept of "anything" that can decide how an entity of its position acts on the board.
    /// </summary>
    public interface IAgent
    {
        Faction Faction { get; }

        UniTask<IDecision?> GetDecision(CancellationToken ct);
    }
}
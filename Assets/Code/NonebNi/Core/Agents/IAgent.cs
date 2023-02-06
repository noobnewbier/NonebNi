using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Decision;

namespace NonebNi.Core.Agents
{
    /// <summary>
    /// An "Agent" is an abstract concept of "anything" that can decide how an entity of its position acts on the board. 
    /// </summary>
    public interface IAgent
    {
        string FactionId { get; }

        UniTask<IDecision?> GetDecision(CancellationToken ct);
    }
}
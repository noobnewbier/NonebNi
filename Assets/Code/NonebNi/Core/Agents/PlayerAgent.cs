using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Decisions;
using NonebNi.Core.Factions;
using Unity.Logging;

namespace NonebNi.Core.Agents
{
    public interface IPlayerAgent : IAgent
    {
        //todo: eventually maybe there should be a "brain"(ai brain, vs player brain), and we can only have one type of agent which would be nice 
        void SetDecision(IDecision decision);
    }

    /// <summary>
    ///     An <see cref="IAgent" /> that delegates its decision making to outside(e.g console, UI) input.
    /// </summary>
    public class PlayerAgent : IPlayerAgent
    {
        private UniTaskCompletionSource<IDecision?>? _tcs;

        public PlayerAgent(Faction faction)
        {
            Faction = faction;
        }

        public Faction Faction { get; }

        public UniTask<IDecision?> GetDecision(CancellationToken ct)
        {
            //if it breaks, well you got what you deserved
            if (_tcs?.UnsafeGetStatus() == UniTaskStatus.Pending)
            {
                // if we are already waiting, just use the same one dude.
                ct.Register(() => _tcs.TrySetResult(null)); // still make sure the ct is propagated though
                return _tcs.Task;
            }

            _tcs = new UniTaskCompletionSource<IDecision?>();
            ct.Register(() => _tcs.TrySetResult(null));
            return _tcs.Task;
        }

        public void SetDecision(IDecision decision)
        {
            if (_tcs == null)
                // if a tree fell in the forest and no one knows
                return;

            var tcs = _tcs;
            _tcs = null; // we want to set this to null before setting the result, that way if there's other chap trying to get decision as the code go on we aren't resetting them

            if (!tcs.TrySetResult(decision)) Log.Error("woah dude this should not have happened");
        }
    }
}
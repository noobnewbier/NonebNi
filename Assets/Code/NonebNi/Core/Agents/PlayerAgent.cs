using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.Logs.Runtime;
using NonebNi.Core.Decisions;
using NonebNi.Core.Factions;

namespace NonebNi.Core.Agents
{
    //todo: reintroduce iplayeragent so the di works again, we cannot have ID based injection and for shit we know in compile time spending over an hour pondering about is too much,
    //you need to get shit done if you want your game to be finished! Or just use a container if that makes you feel better so at least you don't have to change your inheritance hierarchy.
    /// <summary>
    ///     An <see cref="IAgent" /> that delegates its decision making to outside(e.g console, UI) input.
    /// </summary>
    public class WaitForExternalInputAgent : IWaitForExternalInputAgent
    {
        private UniTaskCompletionSource<IDecision?>? _tcs;

        public WaitForExternalInputAgent(Faction faction)
        {
            Faction = faction;
        }

        public Faction Faction { get; }

        public UniTask<IDecision?> GetDecision(CancellationToken ct)
        {
            //if it breaks, well you got what you deserved
            if (_tcs?.UnsafeGetStatus() == UniTaskStatus.Pending)
            {
                // if we are already waiting, just use the same one dude. NOTE: we also did this in BehaviourTreeAgent
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

            if (!tcs.TrySetResult(decision)) Log.Error("AI", "woah dude this should not have happened");
        }
    }
}
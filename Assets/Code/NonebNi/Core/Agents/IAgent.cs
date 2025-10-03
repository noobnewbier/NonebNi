using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Decisions;
using NonebNi.Core.Factions;
using Unity.Behavior;
using Unity.Logging;
using UnityEngine;

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

    public interface IWaitForExternalInputAgent : IAgent
    {
        //todo: eventually maybe there should be a "brain"(ai brain, vs player brain), and we can only have one type of agent which would be nice 
        void SetDecision(IDecision decision);
    }

    public class BehaviourTreeAgent : IAgent
    {
        private readonly BehaviorGraph _graph;
        private UniTaskCompletionSource<IDecision?>? _tcs;

        public BehaviourTreeAgent(Faction faction, BehaviorGraph graph)
        {
            _graph = graph;
            Faction = faction;

            var context = Context.CreateInstance(this);
            _graph.BlackboardReference.AddVariable(Context.Name, context);
        }

        public Faction Faction { get; }

        public UniTask<IDecision?> GetDecision(CancellationToken ct)
        {
            _graph.Restart();

            //if it breaks, well you got what you deserved
            if (_tcs?.UnsafeGetStatus() == UniTaskStatus.Pending)
            {
                // if we are already waiting, just use the same one dude. NOTE: we also did this in PlayerAgent
                ct.Register(() => _tcs.TrySetResult(null)); // still make sure the ct is propagated though
                return _tcs.Task;
            }

            _tcs = new ();
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

        /// <summary>
        /// todo: instead inject agent and get context from there..
        ///  Note:
        ///  Only reason this is a scriptable object is so blackboard can maintain reference to it.
        /// </summary>
        public class Context : ScriptableObject
        {
            public static readonly string Name = $"{nameof(Context)}";

            public BehaviourTreeAgent Agent { get; private set; }

            public static Context CreateInstance(BehaviourTreeAgent agent)
            {
                var toReturn = CreateInstance<Context>();
                toReturn.Agent = agent;

                return toReturn;
            }
        }
    }
}
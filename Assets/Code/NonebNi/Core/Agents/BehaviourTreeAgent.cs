using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Decisions;
using NonebNi.Core.Factions;
using Unity.Behavior;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Core.Agents
{
    public class BehaviourTreeAgent : IAgent
    {
        private readonly BehaviorGraph _graph;
        private readonly BehaviorGraphAgent _graphAgentComponent;
        private UniTaskCompletionSource<IDecision?>? _decisionTcs;

        public BehaviourTreeAgent(Faction faction, BehaviorGraph graph)
        {
            _graph = Object.Instantiate(graph);
            Faction = faction;

            var context = Context.CreateInstance(this);
            _graph.BlackboardReference.AddVariable(Context.Name, context);

            _graphAgentComponent = new GameObject($"{faction.Id}Agent").AddComponent<BehaviorGraphAgent>();
            _graphAgentComponent.Graph = _graph;
        }

        public bool IsWaitingDecision
        {
            get
            {
                if (_decisionTcs == null) return false;

                return !_decisionTcs.UnsafeGetStatus().IsCompleted();
            }
        }

        public void Dispose()
        {
            Object.Destroy(_graphAgentComponent);
        }

        public Faction Faction { get; }

        public UniTask<IDecision?> GetDecision(CancellationToken ct)
        {
            // _graph.Restart();

            //if it breaks, well you got what you deserved
            if (_decisionTcs?.UnsafeGetStatus() == UniTaskStatus.Pending)
            {
                // if we are already waiting, just use the same one dude. NOTE: we also did this in PlayerAgent
                ct.Register(() => _decisionTcs.TrySetResult(null)); // still make sure the ct is propagated though
                return _decisionTcs.Task;
            }

            _decisionTcs = new ();
            ct.Register(() => _decisionTcs.TrySetResult(null));
            return _decisionTcs.Task;
        }

        public void SetDecision(IDecision decision)
        {
            if (_decisionTcs == null)
                // if a tree fell in the forest and no one knows
                return;

            var tcs = _decisionTcs;
            _decisionTcs = null; // we want to set this to null before setting the result, that way if there's other chap trying to get decision as the code go on we aren't resetting them

            if (!tcs.TrySetResult(decision)) Log.Error("woah dude this should not have happened");
        }

        /// <summary>
        ///  Note:
        ///  Only reason this is a scriptable object is so blackboard can maintain reference to it.
        /// </summary>
        public class Context : ScriptableObject
        {
            public static readonly string Name = $"{nameof(Context)}";

            public BehaviourTreeAgent Agent { get; private set; } = null!;

            public static Context CreateInstance(BehaviourTreeAgent agent)
            {
                var toReturn = CreateInstance<Context>();
                toReturn.Agent = agent;

                return toReturn;
            }
        }
    }
}
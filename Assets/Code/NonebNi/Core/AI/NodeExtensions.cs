using NonebNi.BehaviourGraphHacks;
using NonebNi.Core.Agents;
using Unity.Behavior;

namespace NonebNi.Core.AI
{
    public static class NodeExtensions
    {
        public static BehaviourTreeAgent.Context? GetContext(this Node node)
        {
            var blackboard = node.GetBlackboardReference();
            if (!blackboard.GetVariable(BehaviourTreeAgent.Context.Name, out BlackboardVariable<BehaviourTreeAgent.Context> variable)) return null;

            return variable.Value;
        }
    }
}
using Unity.Behavior;

// ReSharper disable once CheckNamespace - stop rider from complaining this is not from Unity!f
namespace NonebNi.BehaviourGraphHacks
{
    public static class GiveNodeAccessToBlackboard
    {
        public static BlackboardReference GetBlackboardReference(this Node node)
        {
            /*
             * Note:
             * Graph is internal as of 1.0.12 - bridge to
             * https://github.com/needle-mirror/com.unity.behavior/blob/master/Runtime/Execution/Node.cs#L82
             */
            var graph = node.Graph;
            return graph.BlackboardReference;
        }
    }
}
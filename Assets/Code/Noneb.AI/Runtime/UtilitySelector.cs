using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Behavior;
using Unity.Logging;
using Unity.Properties;
using UnityUtils;
using Composite = Unity.Behavior.Composite;

namespace Noneb.AI.Runtime
{
    [Serializable, GeneratePropertyBag, NodeDescription("UtilitySelector", story: "Rank all child node by utility, and choose action with highest score", category: "Flow", id: "9c02501079be008cffe3e53c73538a98")]
    public partial class UtilitySelector : Composite
    {
        // only implementing highest for now. it's easiest to debug

        //todo:selector strategy - highest, weighted, weighted and trim
        //todo: inertia?
        //todo: nice to have - editor utility to flag misconfig where one of the child isn't IUtilityNode
        //note: subgraph to group action planning, root node to return utility?
        //note: need an overall tactician/strategy for agent -> strategy decide what the team do -> agent does its part 
        private Node? _currentChild;
        private UtilityFormula _utilityFormula = null!;

        protected override Status OnStart()
        {
            _currentChild = SelectCurrentChild();
            if (_currentChild == null) return Status.Success;

            return StartNode(_currentChild);
        }

        protected override Status OnUpdate()
        {
            if (_currentChild == null) return Status.Success;

            return _currentChild.CurrentStatus;
        }

        private Node? SelectCurrentChild()
        {
            if (!Children.Any()) return null;

            var nodeScores = new Dictionary<Node, float>();

            foreach (var child in Children)
            {
                if (child is not IUtilityNode utilityNode)
                {
                    // negative 1 so any utility node is prioritized.
                    nodeScores[child] = -1f;

                    Log.Error($"Found a node({child.GetType()}) that's not IUtilityNode - likely misconfigured?");
                    continue;
                }

                var utility = utilityNode.FindUtility();
                nodeScores[child] = _utilityFormula.Calculate(utility);
            }

            var bestNode = nodeScores.MaxBy(k => k.Value).Key;
            return bestNode;
        }
    }
}
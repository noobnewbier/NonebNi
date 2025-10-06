using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Agents;
using NonebNi.Core.Commands;
using NonebNi.Core.Decisions;
using NonebNi.Core.FlowControl;
using NonebNi.Core.GameContexts;
using Unity.Behavior;
using Unity.Logging;
using Unity.Properties;
using UnityEngine;
using UnityUtils;

namespace NonebNi.Core.AI
{
    /// <summary>
    /// Note
    /// - Behaviour doesn't support custom variable type by default.
    /// - Way to extend blackboard editor - asmdef linking. Extend TypedVariableElement? Not sure if it's gonna work.
    /// </summary>
    [Serializable, GeneratePropertyBag, NodeDescription("UtilitySelector", story: "Pick child according to [utilityFormula]", category: "Flow", id: "9c02501079be008cffe3e53c73538a98")]
    public partial class UtilitySelector : Composite
    {
        [SerializeReference] public BlackboardVariable<UtilityFormula> utilityFormula = new ();
        private BehaviourTreeAgent.Context? _context;
        // only implementing highest for now. it's easiest to debug

        //todo:selector strategy - highest, weighted, weighted and trim
        //todo: inertia?
        //todo: nice to have - editor utility to flag misconfig where one of the child isn't IUtilityNode
        //note: subgraph to group action planning, root node to return utility?
        //note: need an overall tactician/strategy for agent -> strategy decide what the team do -> agent does its part 

        private AsyncRunner<Dependencies> _dependenciesFetcher = null!;
        private Dependencies _deps = null!;
        private AsyncRunner<ActionCommand?> _selectTaskRunner = null!;

        protected override Status OnStart()
        {
            _selectTaskRunner = new (SelectBestAction);
            _dependenciesFetcher = new (GameContext.Get<Dependencies>);

            return Status.Success;
        }

        protected override Status OnUpdate()
        {
            _context = this.GetContext();
            if (_context == null) return Status.Running;

            {
                var (success, value) = _dependenciesFetcher.GetResult();
                if (!success) return Status.Running;

                _deps = value!;
            }

            IDecision decision;
            {
                var (success, actionCommand) = _selectTaskRunner.GetResult();
                if (!success) return Status.Running;

                decision = actionCommand == null ?
                    new EndTurnDecision() :
                    new ActionDecision(actionCommand);
            }


            _context.Agent.SetDecision(decision);
            return Status.Success;
        }

        private async UniTask<ActionCommand?> SelectBestAction()
        {
            var unit = _deps.UnitTurnOrderer.CurrentUnit;

            if (unit.FactionId != _context!.Agent.Faction.Id)
            {
                Log.Error($"This should've never happened, you are asking agent for {_context.Agent.Faction.Id} to control unit({unit.Name}) from {_context.Agent.Faction.Id}");
                return null;
            }

            if (!Children.Any())
            {
                Log.Warning("This utility selector has no options to pick from.");
                return null;
            }

            var actionUtility = new Dictionary<ActionCommand, Utility>();

            foreach (var child in Children)
            {
                if (child is not IUtilityNode utilityNode)
                {
                    Log.Error($"Found a node({child.GetType()}) that's not IUtilityNode - likely misconfigured?");
                    continue;
                }

                // I have a bad feeling about the action aggregation, can it mess up the maths?
                await foreach (var (action, utility) in utilityNode.FindActionAndUtility(unit))
                {
                    var accumulatedUtility = actionUtility.GetValueOrDefault(action, () => new ());
                    accumulatedUtility += utility;
                    actionUtility[action] = accumulatedUtility;
                }
            }

            var bestAction = actionUtility.MaxBy(t => utilityFormula.Value.Calculate(t.Value)).Key;
            return bestAction;
        }

        /// <summary>
        /// Unity's node runs on an update loop, which by nature fight against async method signature.
        /// This is our way to busy wait for async stuffs in an update loop, by god I hope you find a better way.
        /// </summary>
        private class AsyncRunner<T>
        {
            public delegate UniTask<T> TaskSignature();

            private readonly TaskSignature _toRun;

            private UniTask<T>? _runningTask;
            private T? _value;

            public AsyncRunner(TaskSignature toRun)
            {
                _toRun = toRun;
            }

            public (bool success, T? _value) GetResult()
            {
                // start the task if it's not already running
                _runningTask ??= StartTask();

                // return the result as status, allow busy waiting in update loop.
                return _runningTask.Value.Status switch
                {
                    UniTaskStatus.Succeeded => (true, _value),
                    UniTaskStatus.Faulted => (true, default),
                    UniTaskStatus.Canceled => (true, default),
                    _ => (false, default)
                };

                async UniTask<T> StartTask()
                {
                    _value = await _toRun();
                    return _value;
                }
            }
        }

        public record Dependencies(IUnitTurnOrderer UnitTurnOrderer);
    }
}
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.Effects;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using NonebNi.Core.Stats;
using NonebNi.Core.Units;
using Unity.Logging;

namespace NonebNi.Core.FlowControl
{
    public interface IActionCommandEvaluator
    {
        IEnumerable<ISequence> Evaluate(ActionCommand command);
        EffectContext FindEffectContext(ActionCommand command);
        IEnumerable<StatCost> FindActionCostInCurrentState(NonebAction action);
    }

    public class ActionCommandEvaluator : IActionCommandEvaluator
    {
        //TODO: maybe a list for priority..? but priority should be baked within the class no?
        private readonly IReadOnlyCollection<IEffectEvaluator> _evaluators;
        private readonly IGameEventControl _gameEventControl;
        private readonly IMap _map;
        private readonly ITargetFinder _targetFinder;

        public ActionCommandEvaluator(IMap map, ITargetFinder targetFinder, IReadOnlyCollection<IEffectEvaluator> evaluators, IGameEventControl gameEventControl)
        {
            _map = map;
            _targetFinder = targetFinder;
            _evaluators = evaluators;
            _gameEventControl = gameEventControl;
        }

        public IEnumerable<StatCost> FindActionCostInCurrentState(NonebAction action)
        {
            foreach (var c in action.Costs)
            {
                var cost = c;
                if (_gameEventControl.IsEvaluatingCombo)
                    switch (cost.StatId)
                    {
                        case StatId.ActionPoint:
                            // Don't need to pay action point on combo
                            continue;

                        case StatId.Fatigue:

                            // gameplay logic -> fatigue requirement is divided by 2 
                            cost /= 2;
                            break;
                    }

                yield return cost;
            }
        }

        public IEnumerable<ISequence> Evaluate(ActionCommand command)
        {
            //todo: wbn if decision validator can be baked into this?
            if (command.Action.Costs.Any())
            {
                if (command.ActorEntity is not UnitData unitData)
                    Log.Warning($"{command.ActorEntity} is not an unit - cannot pay cost for {command.Action} - we are still doing it though");
                else
                {
                    foreach (var cost in FindActionCostInCurrentState(command.Action))
                    {
                        unitData.Stats.PayCost(cost);
                    }
                }
            }

            return command.Action.Effects.SelectMany(
                e =>
                {
                    var context = FindEffectContext(command);

                    var (isSuccess, sequences) = Evaluate(e, context);
                    if (!isSuccess) Log.Error($"Cannot find evaluator that can handle ({e.GetType()})");

                    return sequences;
                }
            );
        }

        public EffectContext FindEffectContext(ActionCommand command)
        {
            var requests = command.Action.TargetRequests;
            var targetGroups = new List<EffectTargetGroup>();
            for (var i = 0; i < requests.Length; i++)
            {
                var targetCoord = command.TargetCoords[i];
                var targetRequest = requests[i];
                var restriction = targetRequest.TargetRestrictionFlags;
                var targetArea = targetRequest.TargetArea;

                var targets = _targetFinder.FindTargets(command.ActorEntity, targetCoord, targetArea, restriction);
                var group = new EffectTargetGroup(targets.ToArray());

                targetGroups.Add(group);
            }

            return new EffectContext(_map, command, targetGroups);
        }

        private (bool isSuccess, IEnumerable<ISequence> sequences) Evaluate(Effect e, EffectContext context)
        {
            foreach (var evaluator in _evaluators)
            {
                var (isSuccess, sequences) = evaluator.Evaluate(e, context);
                if (isSuccess) return (true, sequences);
            }

            return (false, Enumerable.Empty<ISequence>());
        }
    }
}
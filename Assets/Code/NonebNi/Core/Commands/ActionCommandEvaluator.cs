using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.Effects;
using NonebNi.Core.Maps;
using NonebNi.Core.Stats;
using NonebNi.Core.Units;
using Unity.Logging;

namespace NonebNi.Core.FlowControl
{
    public interface IActionCommandEvaluator
    {
        EffectResult Evaluate(ActionCommand command);
        EffectContext FindEffectContext(ActionCommand command);
        IEnumerable<StatCost> FindActionCostInCurrentState(ActionCommand command);
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

        public IEnumerable<StatCost> FindActionCostInCurrentState(ActionCommand command)
        {
            foreach (var req in command.Action.StatRequirements)
            {
                var cost = req.CalculateCost(command, _map);
                if (_gameEventControl.ActiveActionResult.CanCombo)
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

        public EffectResult Evaluate(ActionCommand command)
        {
            //todo: wbn if decision validator can be baked into this?
            if (command.Action.StatRequirements.Any())
            {
                if (command.ActorEntity is not UnitData unitData)
                    Log.Warning($"{command.ActorEntity} is not an unit - cannot pay cost for {command.Action} - we are still doing it though");
                else
                {
                    foreach (var cost in FindActionCostInCurrentState(command))
                    {
                        unitData.Stats.PayCost(cost);
                    }
                }
            }

            var results = command.Action.Effects.Select
                                 (e =>
                                     {
                                         var context = FindEffectContext(command);

                                         var (isSuccess, result) = Evaluate(e, context);
                                         if (!isSuccess) Log.Error($"Cannot find evaluator that can handle ({e.GetType()})");

                                         return result;
                                     }
                                 )
                                 .ToArray();

            var resultAggregate = results.Aggregate((a, b) => a.Concat(b));
            return resultAggregate;
        }

        public EffectContext FindEffectContext(ActionCommand command)
        {
            /*
             * Note:
             * There might be a day where we need to let our NonebAction define how the user's input for each request is "piped" into the effect evaluation.
             * For example, Request[0] might be piping input to both Effect[0] and Effect[1], while Request[1] only pipes to Effect[2].
             *
             * With our current implementation, there's a fundamental issue where there's no way for an effect to know what they are working with,
             * this plus the request also having no idea meaning trying to make all effect works all the time is virtually impossible.
             *
             * As the action is the only one who has the info on how each request is interacting with each effect, that's the only way to handle it somewhat elegantly.
             * We might want a graph editor for this, as trying to input it as text makes me puke.
             *
             * A simpler approach is to hard code more effects, which can of course tailor to the action itself so we essentially always know what we are working with.
             * which honestly, might not be a bad idea(perhaps easier on the animation handling as well?).
             * The caveat is that there will be less code sharing and might be trouble later, we shall see what do we do down the line.
             */
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

        private (bool isSuccess, EffectResult result) Evaluate(Effect e, EffectContext context)
        {
            foreach (var evaluator in _evaluators)
            {
                var (isSuccess, sequences) = evaluator.Evaluate(e, context);
                if (isSuccess) return (true, sequences);
            }

            return (false, EffectResult.Empty);
        }
    }
}
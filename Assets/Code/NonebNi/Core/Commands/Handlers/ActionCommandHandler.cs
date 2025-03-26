using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Effects;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using Unity.Logging;

namespace NonebNi.Core.Commands.Handlers
{
    public class ActionCommandHandler : ICommandHandler<ActionCommand>
    {
        //TODO: maybe a list for priority..? but priority should be baked within the class no?
        private readonly IReadOnlyCollection<IEffectEvaluator> _evaluators;
        private readonly IMap _map;
        private readonly ITargetFinder _targetFinder;

        public ActionCommandHandler(IMap map, ITargetFinder targetFinder, IReadOnlyCollection<IEffectEvaluator> evaluators)
        {
            _map = map;
            _targetFinder = targetFinder;
            _evaluators = evaluators;
        }

        public IEnumerable<ISequence> Evaluate(ActionCommand command)
        {
            return command.Action.Effects.SelectMany(
                e =>
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

                    var context = new EffectContext(_map, command.ActorEntity, targetGroups);

                    var (isSuccess, sequences) = Evaluate(e, context);
                    if (!isSuccess) Log.Error($"Cannot find evaluator that can handle ({e.GetType()})");

                    return sequences;
                }
            );
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
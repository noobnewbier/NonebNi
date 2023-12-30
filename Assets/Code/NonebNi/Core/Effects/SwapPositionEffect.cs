using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using Unity.Logging;

namespace NonebNi.Core.Effects
{
    public class SwapPositionEffect : Effect
    {
        protected override IEnumerable<ISequence> OnEvaluate(
            IMap map,
            EntityData actionCaster,
            IEnumerable<IActionTarget> targets)
        {
            var targetParam = targets.FirstOrDefault();
            if (targetParam is not EntityData targetEntity)
            {
                Log.Error($"{nameof(SwapPositionEffect)} without an Entity parameter makes no sense!");
                yield break;
            }

            if (!map.TryFind(actionCaster, out Coordinate actorCoord))
            {
                Log.Error($"{actionCaster.Name} is not on the map!");
                yield break;
            }

            if (!map.TryFind(targetEntity, out Coordinate targetCoord))
            {
                Log.Error($"{targetEntity.Name} is not on the map!");
                yield break;
            }

            map.Remove(targetEntity);
            map.Remove(actionCaster);

            map.Put(targetCoord, actionCaster);
            map.Put(actorCoord, targetEntity);

            yield return new AggregateSequence(
                new MoveSequence(targetEntity, actorCoord),
                new MoveSequence(actionCaster, targetCoord)
            );
        }
    }
}
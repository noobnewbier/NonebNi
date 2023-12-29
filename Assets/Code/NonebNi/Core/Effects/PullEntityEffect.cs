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
    public class PullEntityEffect : Effect
    {
        protected override IEnumerable<ISequence> OnEvaluate(
            IMap map,
            EntityData actionCaster,
            IEnumerable<IActionTarget> targets)
        {
            var targetParam = targets.FirstOrDefault();
            if (targetParam is not EntityData targetEntity)
            {
                Log.Error($"{nameof(PullEntityEffect)} without an Entity parameter makes no sense!");
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

            if (!actorCoord.IsOnSameLineWith(targetCoord))
            {
                Log.Error(
                    $"{targetEntity.Name} is not on the same line with {actionCaster.Name} - effect is undefined!"
                );
                yield break;
            }

            var direction = (targetCoord - actorCoord).Normalized();
            var pulledToCoord = actorCoord + direction;
            var result = map.Move(targetEntity, pulledToCoord);
            if (result != MoveResult.Success)
            {
                Log.Warning($"Failed movement! Reason: {result}.");
                yield break;
            }

            yield return new MoveSequence(targetEntity, pulledToCoord);
        }
    }
}
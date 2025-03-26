using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using Unity.Logging;

namespace NonebNi.Core.Effects
{
    public class PullEntityEffect : Effect
    {
        public class Evaluator : Evaluator<PullEntityEffect>
        {
            protected override IEnumerable<ISequence> OnEvaluate(
                PullEntityEffect effect,
                EffectContext context)
            {
                var targetParam = context.TargetGroups.FirstOrDefault()?.AsSingleTarget;
                if (targetParam is not EntityData targetEntity)
                {
                    Log.Error($"{nameof(PullEntityEffect)} without an Entity parameter makes no sense!");
                    yield break;
                }

                if (!context.Map.TryFind(context.ActionCaster, out Coordinate actorCoord))
                {
                    Log.Error($"{context.ActionCaster.Name} is not on the map!");
                    yield break;
                }

                if (!context.Map.TryFind(targetEntity, out Coordinate targetCoord))
                {
                    Log.Error($"{targetEntity.Name} is not on the map!");
                    yield break;
                }

                if (!actorCoord.IsOnSameLineWith(targetCoord))
                {
                    Log.Error(
                        $"{targetEntity.Name} is not on the same line with {context.ActionCaster.Name} - effect is undefined!"
                    );
                    yield break;
                }

                var direction = (targetCoord - actorCoord).Normalized();
                var pulledToCoord = actorCoord + direction;
                var result = context.Map.Move(targetEntity, pulledToCoord);
                if (result != MoveResult.Success)
                {
                    Log.Warning($"Failed movement! Reason: {result}.");
                    yield break;
                }

                yield return new MoveSequence(targetEntity, pulledToCoord);
            }
        }
    }
}
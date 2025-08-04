using System;
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
    [Serializable]
    public class PullEntityEffect : Effect
    {
        public class Evaluator : Evaluator<PullEntityEffect>
        {
            protected override EffectResult OnEvaluate(
                PullEntityEffect effect,
                EffectContext context)
            {
                var targetParam = context.TargetGroups.FirstOrDefault()?.AsSingleTarget;
                if (targetParam is not EntityData targetEntity)
                {
                    Log.Error($"{nameof(PullEntityEffect)} without an Entity parameter makes no sense!");
                    return EffectResult.Empty;
                }

                if (!context.Map.TryFind(context.ActionCaster, out Coordinate actorCoord))
                {
                    Log.Error($"{context.ActionCaster.Name} is not on the map!");
                    return EffectResult.Empty;
                }

                if (!context.Map.TryFind(targetEntity, out Coordinate targetCoord))
                {
                    Log.Error($"{targetEntity.Name} is not on the map!");
                    return EffectResult.Empty;
                }

                if (!actorCoord.IsOnSameLineWith(targetCoord))
                {
                    Log.Error(
                        $"{targetEntity.Name} is not on the same line with {context.ActionCaster.Name} - effect is undefined!"
                    );
                    return EffectResult.Empty;
                }

                var direction = (targetCoord - actorCoord).Normalized();
                var pulledToCoord = actorCoord + direction;
                var result = context.Map.Move(targetEntity, pulledToCoord);
                if (result != MoveResult.Success)
                {
                    Log.Warning($"Failed movement! Reason: {result}.");
                    return EffectResult.Empty;
                }

                var sequences = new List<ISequence>();
                var receivers = new HashSet<IActionTarget>();
                var carriers = new HashSet<EntityData>();
                if (targetEntity.FactionId == context.ActionCaster.FactionId)
                    carriers.Add(targetEntity);
                else
                    receivers.Add(targetEntity);

                sequences.Add(new MoveSequence(targetEntity, pulledToCoord));
                return new EffectResult(sequences, receivers, carriers);
            }
        }
    }
}
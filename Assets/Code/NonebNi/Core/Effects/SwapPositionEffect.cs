using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.Sequences;
using Unity.Logging;

namespace NonebNi.Core.Effects
{
    [Serializable]
    public class SwapPositionEffect : Effect
    {
        public override bool IsEffectNegative => false;

        public class Evaluator : Evaluator<SwapPositionEffect>
        {
            protected override EffectResult OnEvaluate(
                SwapPositionEffect effect,
                EffectContext context)
            {
                var targetParam = context.TargetGroups.FirstOrDefault()?.AsSingleTarget;
                if (targetParam is not EntityData targetEntity)
                {
                    Log.Error($"{nameof(SwapPositionEffect)} without an Entity parameter makes no sense!");
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

                context.Map.Remove(targetEntity);
                context.Map.Remove(context.ActionCaster);

                context.Map.Put(targetCoord, context.ActionCaster);
                context.Map.Put(actorCoord, targetEntity);

                var sequences = new List<ISequence>();
                var receivers = new HashSet<IActionTarget>();
                var carriers = new HashSet<EntityData>();
                if (targetEntity.FactionId == context.ActionCaster.FactionId)
                    carriers.Add(targetEntity);
                else
                    receivers.Add(targetEntity);

                sequences.Add(
                    new AggregateSequence(
                        new MoveSequence(targetEntity, actorCoord),
                        new MoveSequence(context.ActionCaster, targetCoord)
                    )
                );

                return new EffectResult(sequences, receivers, carriers);
            }
        }
    }
}
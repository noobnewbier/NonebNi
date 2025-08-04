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
    public class MoveEntityEffect : Effect
    {
        public class Evaluator : Evaluator<MoveEntityEffect>
        {
            protected override EffectResult OnEvaluate(
                MoveEntityEffect effect,
                EffectContext context)
            {
                var groups = context.TargetGroups.ToArray();
                if (groups.Count() != 2)
                    Log.Error($"{nameof(MoveEntityEffect)} expects 2 and exactly 2 parameter - something went wrong.");

                if (groups.FirstOrDefault()?.AsSingleTarget is not EntityData targetEntity)
                {
                    Log.Error($"{nameof(MoveEntityEffect)} without an entity to move makes no sense!");
                    return EffectResult.Empty;
                }

                if (groups.ElementAtOrDefault(1)?.AsSingleTarget is not Coordinate targetCoord)
                {
                    Log.Error($"{nameof(MoveEntityEffect)} without a coordinate as target position makes no sense!");
                    return EffectResult.Empty;
                }

                var result = context.Map.Move(targetEntity, targetCoord);

                if (result != MoveResult.Success) return EffectResult.Empty;

                var sequences = new List<ISequence>();
                var receivers = new HashSet<IActionTarget>();
                var carriers = new HashSet<EntityData>();
                if (targetEntity.FactionId == context.ActionCaster.FactionId)
                    carriers.Add(targetEntity);
                else
                    receivers.Add(targetEntity);

                sequences.Add(new MoveSequence(targetEntity, targetCoord));

                return new EffectResult(sequences, receivers, carriers);
            }
        }
    }
}
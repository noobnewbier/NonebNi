using System;
using System.Collections.Generic;
using System.Linq;
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
            protected override IEnumerable<ISequence> OnEvaluate(
                MoveEntityEffect effect,
                EffectContext context)
            {
                var groups = context.TargetGroups.ToArray();
                if (groups.Count() != 2)
                    Log.Error($"{nameof(MoveEntityEffect)} expects 2 and exactly 2 parameter - something went wrong.");

                if (groups.FirstOrDefault()?.AsSingleTarget is not EntityData targetEntity)
                {
                    Log.Error($"{nameof(MoveEntityEffect)} without an entity to move makes no sense!");
                    yield break;
                }

                if (groups.ElementAtOrDefault(1)?.AsSingleTarget is not Coordinate targetCoord)
                {
                    Log.Error($"{nameof(MoveEntityEffect)} without a coordinate as target position makes no sense!");
                    yield break;
                }

                var result = context.Map.Move(targetEntity, targetCoord);

                if (result == MoveResult.Success) yield return new MoveSequence(targetEntity, targetCoord);
            }
        }
    }
}
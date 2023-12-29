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
        protected override IEnumerable<ISequence> OnEvaluate(
            IMap map,
            EntityData actionCaster,
            IEnumerable<IActionTarget> targets)
        {
            targets = targets.ToArray();
            if (targets.Count() != 2)
                Log.Error($"{nameof(MoveEntityEffect)} expects 2 and exactly 2 parameter - something went wrong.");

            if (targets.FirstOrDefault() is not EntityData targetEntity)
            {
                Log.Error($"{nameof(MoveEntityEffect)} without an entity to move makes no sense!");
                yield break;
            }

            if (targets.ElementAtOrDefault(1) is not Coordinate targetCoord)
            {
                Log.Error($"{nameof(MoveEntityEffect)} without a coordinate as target position makes no sense!");
                yield break;
            }

            var result = map.Move(targetEntity, targetCoord);

            if (result == MoveResult.Success) yield return new MoveSequence(targetEntity, targetCoord);
        }
    }
}
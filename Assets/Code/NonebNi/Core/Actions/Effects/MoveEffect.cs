using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using Unity.Logging;

namespace NonebNi.Core.Actions.Effects
{
    [Serializable]
    public class MoveEffect : Effect
    {
        protected override IEnumerable<ISequence> OnEvaluate(
            IMap map,
            EntityData actionCaster,
            IEnumerable<IActionTarget> targets)
        {
            var targetParam = targets.FirstOrDefault();
            if (targetParam is not Coordinate targetCoord)
            {
                Log.Info(
                    "{targetParam} is not a Coordinate - MoveEffect must takes one coordinate as parameter!",
                    targetParam
                );
                yield break;
            }

            var result = map.Move(actionCaster, targetCoord);

            if (result == MoveResult.Success) yield return new MoveSequence(actionCaster, targetCoord);
        }
    }
}
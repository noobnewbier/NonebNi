using System;
using System.Collections.Generic;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using NonebNi.Core.Units;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Core.Actions.Effects
{
    [Serializable]
    public class KnockBackEffect : Effect
    {
        private readonly int _range;

        public KnockBackEffect(int range)
        {
            _range = range;
        }

        protected override IEnumerable<ISequence> OnEvaluate(
            IMap map,
            EntityData actionCaster,
            IEnumerable<IActionTarget> targets)
        {
            if (!map.TryFind(actionCaster, out Coordinate casterCoord))
            {
                Log.Error($"[Effect] {actionCaster} does not exist in the map!");
                yield break;
            }

            foreach (var target in targets)
            {
                var targetUnit = target as UnitData;
                var maybeTargetOriginCoord = target as Coordinate?;
                if (targetUnit == null && maybeTargetOriginCoord.HasValue)
                    if (!map.TryGet(maybeTargetOriginCoord.Value, out targetUnit))
                    {
                        Log.Error($"[Effect] Cannot find unit in given coordinate({nameof(target)})!", target);
                        continue;
                    }

                if (!maybeTargetOriginCoord.HasValue && targetUnit != null)
                {
                    if (!map.TryFind(targetUnit, out Coordinate foundTargetOriginCoord))
                    {
                        Debug.LogError($"[Effect] Target({targetUnit}) does not exist in the map!");
                        continue;
                    }

                    maybeTargetOriginCoord = foundTargetOriginCoord;
                }

                if (targetUnit == null || !maybeTargetOriginCoord.HasValue)
                {
                    Log.Error($"[Effect] Unexpected target type({target.GetType()}), cannot resolve given parameter!");
                    continue;
                }


                var knockBackDirection = (maybeTargetOriginCoord.Value - casterCoord).Normalized();
                var finalCoord = maybeTargetOriginCoord.Value + knockBackDirection * _range;
                var moveResult = map.Move(targetUnit, finalCoord);
                switch (moveResult)
                {
                    case MoveResult.Success:
                    case MoveResult.NoEffect:
                    case MoveResult.ErrorTargetOccupied:
                        yield return new KnockBackSequence(targetUnit, finalCoord);
                        break;

                    case MoveResult.ErrorNoEntityToBeMoved:
                        Log.Error($"[Effect] Target({targetUnit}) does not exist in the map!");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}
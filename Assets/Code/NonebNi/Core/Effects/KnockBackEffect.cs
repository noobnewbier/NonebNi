using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Core.Effects
{
    [Serializable]
    public class KnockBackEffect : Effect
    {
        [SerializeField] private int distance;

        public KnockBackEffect(int distance)
        {
            this.distance = distance;
        }

        public class Evaluator : Evaluator<KnockBackEffect>
        {
            protected override IEnumerable<ISequence> OnEvaluate(
                KnockBackEffect effect,
                EffectContext context)
            {
                if (!context.Map.TryFind(context.ActionCaster, out Coordinate casterCoord))
                {
                    Log.Error($"[Effect] {context.ActionCaster} does not exist in the map!");
                    yield break;
                }

                foreach (var target in context.TargetGroups.SelectMany(g => g.Targets))
                {
                    if (target is not EntityData targetEntity)
                    {
                        Log.Error($"[Effect] Unexpected target type({target.GetType()}), cannot resolve given parameter!");
                        continue;
                    }

                    if (!context.Map.TryFind(targetEntity, out IEnumerable<Coordinate> targetCoords))
                    {
                        Log.Error($"[Effect] Target({targetEntity}) does not exist in the map!");
                        continue;
                    }

                    targetCoords = targetCoords.ToArray();
                    if (targetCoords.Count() > 1)
                    {
                        Log.Error(
                            $"[Effect] Target({targetEntity}) spans across more than one tile! This is not supported at the moment"
                        );
                        continue;
                    }

                    var targetOriginCoord = targetCoords.First();
                    var knockBackDirection = (targetOriginCoord - casterCoord).Normalized();
                    var finalCoord = GetCoordinateAfterKnockBack(effect, context.Map, targetOriginCoord, knockBackDirection);
                    var moveResult = context.Map.Move(targetEntity, finalCoord);
                    switch (moveResult)
                    {
                        case MoveResult.Success:
                        case MoveResult.NoEffect:
                        case MoveResult.ErrorTargetOccupied:
                            yield return new KnockBackSequence(targetEntity, finalCoord);
                            break;

                        case MoveResult.ErrorEntityIsNotOnBoard:
                            Log.Error($"[Effect] Target({targetEntity}) does not exist in the map!");
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            private Coordinate GetCoordinateAfterKnockBack(
                KnockBackEffect effect,
                IReadOnlyMap map,
                Coordinate targetOriginCoord,
                Coordinate knockBackDirection)
            {
                var finalCoord = targetOriginCoord;

                //Find the furthest, non occupied coordinate. Any obstruction within knock back path blocks the knock back.
                for (var i = 1; i < effect.distance + 1; i++)
                {
                    var coordInKnockBackPath = targetOriginCoord + knockBackDirection * i;
                    if (!map.IsCoordinateWithinMap(coordInKnockBackPath)) break;

                    if (map.IsOccupied(coordInKnockBackPath)) break;

                    finalCoord = coordInKnockBackPath;
                }

                //If every coordinate along the way is occupied - stay in its place.
                return finalCoord;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;
using Unity.Logging;
using UnityUtils;

namespace NonebNi.Core.Actions
{
    public interface ITargetFinder
    {
        IEnumerable<IActionTarget> FindTargets(
            EntityData actor,
            IReadOnlyList<Coordinate> targetCoords,
            TargetArea targetArea,
            TargetRestriction[] targetRestrictions);

        IEnumerable<IActionTarget> FindTargets(
            EntityData actor,
            Coordinate targetCoord,
            TargetArea targetArea,
            TargetRestriction restriction);
    }

    public class TargetFinder : ITargetFinder
    {
        private readonly IReadOnlyMap _map;

        public TargetFinder(IReadOnlyMap map)
        {
            _map = map;
        }

        public IEnumerable<IActionTarget> FindTargets(
            EntityData actor,
            IReadOnlyList<Coordinate> targetCoords,
            TargetArea targetArea,
            TargetRestriction[] targetRestrictions)
        {
            for (var i = 0; i < targetCoords.Count; i++)
            {
                var targetCoord = targetCoords[i];
                var restriction = targetRestrictions[i];
                foreach (var actionTarget in FindTargets(actor, targetCoord, targetArea, restriction))
                    yield return actionTarget;
            }
        }

        public IEnumerable<IActionTarget> FindTargets(
            EntityData actor,
            Coordinate targetCoord,
            TargetArea targetArea,
            TargetRestriction restriction)
        {
            foreach (var coord in GetTargetedCoordinates(actor, targetCoord, targetArea))
            foreach (var target in GetValidTargetsInCoordinate(actor, coord, restriction))
                yield return target;
        }

        private IEnumerable<Coordinate> GetTargetedCoordinates(
            EntityData actor,
            Coordinate targetCoord,
            TargetArea targetArea)
        {
            switch (targetArea)
            {
                case TargetArea.Single:
                    yield return targetCoord;
                    break;
                case TargetArea.Fan:
                    if (actor.IsSystem)
                    {
                        Log.Error($"{TargetArea.Fan} is not supported for System Entity");
                        yield break;
                    }

                    if (_map.TryFind(actor, out Coordinate actorCoord))
                    {
                        Log.Error("Actor is not found on the map. We can't figure out the direction of the fan!");
                        yield break;
                    }

                    var relativeCoord = targetCoord - actorCoord;
                    yield return targetCoord;
                    yield return relativeCoord.RotateLeft();
                    yield return relativeCoord.RotateRight();

                    break;
                case TargetArea.Circle:
                    foreach (var neighbour in targetCoord.Neighbours)
                        if (_map.IsCoordinateWithinMap(neighbour))
                            yield return neighbour;

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetArea), targetArea, null);
            }
        }

        private IEnumerable<IActionTarget> GetValidTargetsInCoordinate(
            EntityData caster,
            Coordinate targetCoord,
            TargetRestriction targetRestrictionFlags)
        {
            foreach (var target in GetAllTargets())
            {
                if (FailedAnyTargetRestrictionCheck(target)) continue;

                yield return target;
            }

            yield break;

            IEnumerable<IActionTarget> GetAllTargets()
            {
                if (_map.TryGet(targetCoord, out IEnumerable<EntityData>? entities))
                {
                    //Stuffs move around when effect is evaluated, this leads to changes in the entities
                    //so we make a copy so we aren't modifying the enumeration in a foreach loop
                    var targetsCopy = entities.ToArray();
                    foreach (var entity in targetsCopy)
                        yield return entity;
                }

                yield return targetCoord;
            }

            bool FailedAnyTargetRestrictionCheck(IActionTarget target)
            {
                return targetRestrictionFlags.GetFlags().Any(f => FailedTargetRestrictionCheck(f, target));
            }

            bool FailedTargetRestrictionCheck(TargetRestriction restriction, IActionTarget target)
            {
                return !IsPassingTargetRestrictionCheck(
                    restriction,
                    caster,
                    target
                );
            }
        }

        private bool IsPassingTargetRestrictionCheck(
            TargetRestriction restriction,
            EntityData caster,
            IActionTarget target)
        {
            var targetCoord = target switch
            {
                Coordinate targetAsCoord => targetAsCoord,
                EntityData entityData when _map.TryFind(entityData, out Coordinate coordinate) => coordinate,

                _ => throw new ArgumentOutOfRangeException(
                    nameof(target),
                    target,
                    "Unexpected Target! Is target even on the board, or did you implement new type but didn't add it here?"
                )
            };

            if (!_map.IsCoordinateWithinMap(targetCoord)) return false;

            if (!_map.TryFind(caster, out Coordinate casterCoord)) return false;

            switch (restriction)
            {
                case TargetRestriction.None:
                    return true;
                case TargetRestriction.Friendly:
                {
                    if (target is not UnitData targetUnit) return false;

                    return caster.FactionId == targetUnit.FactionId;
                }
                case TargetRestriction.Enemy:
                {
                    if (target is not UnitData targetUnit) return false;
                    return caster.FactionId != targetUnit.FactionId;
                }
                case TargetRestriction.NonOccupied:
                {
                    return !_map.IsOccupied(targetCoord);
                }
                case TargetRestriction.ClearPath:
                {
                    if (!casterCoord.IsOnSameLineWith(targetCoord)) return false;

                    if (casterCoord.GetCoordinatesBetween(targetCoord).Any(_map.IsOccupied)) return false;

                    return true;
                }
                case TargetRestriction.Back:
                    throw new NotImplementedException("back defined by another ally..?");
                    break;
                case TargetRestriction.Self:
                    return ReferenceEquals(caster, target);
                case TargetRestriction.Obstacle:
                {
                    var tileModifier = _map.Get<TileModifierData>(targetCoord);
                    return tileModifier is { TileData: { IsWall: true } };
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(restriction), restriction, null);
            }
        }
    }
}
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
            Coordinate targetCoord,
            TargetArea targetArea,
            TargetRestriction restrictionFlags);

        IEnumerable<(RangeStatus status, Coordinate coord)> FindRange(EntityData caster, TargetRequest request);

        IEnumerable<Coordinate> GetTargetedCoordinates(
            EntityData actor,
            Coordinate targetCoord,
            TargetArea targetArea);
    }

    public class TargetFinder : ITargetFinder
    {
        private readonly IReadOnlyMap _map;

        public TargetFinder(IReadOnlyMap map)
        {
            _map = map;
        }

        public IEnumerable<(RangeStatus status, Coordinate coord)> FindRange(EntityData caster, TargetRequest request)
        {
            //todo: sometimes caster coord is null -> why?
            if (!_map.TryFind(caster, out Coordinate casterCoord)) yield break;

            var range = request.Range.CalculateRange(caster);
            var inRangeCoords = GetInRangeCoords(casterCoord, request.TargetRestrictionFlags, range);

            foreach (var coord in inRangeCoords)
            {
                var restrictions = request.TargetRestrictionFlags.GetFlags();
                var failedReasons = new List<RestrictionCheckFailedReason>();
                foreach (var restriction in restrictions)
                {
                    var (passedCheck, reason) = CheckTargetRestriction(restriction, caster, coord);
                    if (passedCheck) continue;
                    failedReasons.Add(reason!);
                }

                if (!failedReasons.Any())
                {
                    yield return (new RangeStatus.Targetable(), coord);
                    continue;
                }

                if (failedReasons.OfType<RestrictionCheckFailedReason.NotOccupied>().Any())
                {
                    yield return (new RangeStatus.InRangeButNoTarget(), coord);
                    continue;
                }

                if (failedReasons.OfType<RestrictionCheckFailedReason.OutOfRange>().Any())
                {
                    yield return (new RangeStatus.OutOfRange(), coord);
                    continue;
                }

                yield return (new RangeStatus.NotTargetable(failedReasons), coord);
            }
        }

        private IEnumerable<Coordinate> GetInRangeCoords(Coordinate actorCoord, TargetRestriction restrictionFlag, int range)
        {
            if (range <= 0) yield break;

            //Moving on if there's any extra "area limitation" range this is where we want to put it, or at least that's what I am thinking now.
            if (restrictionFlag.HasFlag(TargetRestriction.ClearPath) || restrictionFlag.HasFlag(TargetRestriction.FirstTileToTargetDirectionIsEmpty))
                foreach (var dir in HexDirection.All)
                    for (var i = 0; i < range; i++)
                    {
                        var coord = actorCoord + dir * i;
                        if (_map.IsCoordinateWithinMap(coord)) yield return coord;
                    }
            else
                foreach (var coord in actorCoord.WithinDistance(range))
                    if (_map.IsCoordinateWithinMap(coord))
                        yield return coord;

            //If we can target self, at least return ourselves
            if (restrictionFlag.HasFlag(TargetRestriction.Self)) yield return actorCoord;
        }

        private (bool passedCheck, RestrictionCheckFailedReason? reason) CheckTargetRestriction(
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
            var targetEntity = target switch
            {
                EntityData targetAsEntity => targetAsEntity,
                Coordinate targetAsCoord => _map.Get<EntityData>(targetAsCoord),

                _ => null
            };

            if (!_map.IsCoordinateWithinMap(targetCoord)) return (false, new RestrictionCheckFailedReason.TargetNotOnMap());

            if (!_map.TryFind(caster, out Coordinate casterCoord)) return (false, new RestrictionCheckFailedReason.CasterNotOnMap());

            switch (restriction)
            {
                case TargetRestriction.None:
                    return (true, null);
                case TargetRestriction.Friendly:
                {
                    if (targetEntity is not UnitData targetUnit) return (false, new RestrictionCheckFailedReason.TargetTypeNotMatched());
                    if (caster.FactionId != targetUnit.FactionId) return (false, new RestrictionCheckFailedReason.NotFriendly());

                    return (true, null);
                }
                case TargetRestriction.Enemy:
                {
                    if (targetEntity is not UnitData targetUnit) return (false, new RestrictionCheckFailedReason.TargetTypeNotMatched());
                    if (caster.FactionId == targetUnit.FactionId) return (false, new RestrictionCheckFailedReason.NotEnemy());
                    return (true, null);
                }
                case TargetRestriction.NonOccupied:
                {
                    if (_map.IsOccupied(targetCoord)) return (false, new RestrictionCheckFailedReason.Occupied());

                    return (true, null);
                }
                case TargetRestriction.ClearPath:
                {
                    if (!casterCoord.IsOnSameLineWith(targetCoord)) return (false, new RestrictionCheckFailedReason.OutOfRange());
                    if (casterCoord.GetCoordinatesBetween(targetCoord).Any(_map.IsOccupied)) return (false, new RestrictionCheckFailedReason.NotClearPath());

                    return (true, null);
                }
                case TargetRestriction.Back:
                    throw new NotImplementedException("back defined by another ally..?");
                    break;
                case TargetRestriction.Self:
                    if (!ReferenceEquals(caster, target)) return (false, new RestrictionCheckFailedReason.NotSelf());

                    return (true, null);
                case TargetRestriction.Obstacle:
                {
                    var tileModifier = _map.Get<TileModifierData>(targetCoord);
                    if (tileModifier is not { TileData: { IsWall: true } }) return (false, new RestrictionCheckFailedReason.NotObstacle());

                    return (true, null);
                }
                case TargetRestriction.FirstTileToTargetDirectionIsEmpty:
                {
                    if (!casterCoord.IsOnSameLineWith(targetCoord)) return (false, new RestrictionCheckFailedReason.OutOfRange());

                    var direction = (targetCoord - casterCoord).Normalized();
                    var firstTileToTargetDirection = casterCoord + direction;

                    if (!_map.IsCoordinateWithinMap(firstTileToTargetDirection)) return (false, new RestrictionCheckFailedReason.NotInMap());

                    if (_map.IsOccupied(firstTileToTargetDirection)) return (false, new RestrictionCheckFailedReason.NotClearPath());

                    return (true, null);
                }
                case TargetRestriction.TargetCoordPlusDirectionToTargetIsEmpty:
                {
                    if (!casterCoord.IsOnSameLineWith(targetCoord)) return (false, new RestrictionCheckFailedReason.OutOfRange());

                    var direction = (targetCoord - casterCoord).Normalized();
                    var targetCoordPlusDirection = targetCoord + direction;

                    if (!_map.IsCoordinateWithinMap(targetCoordPlusDirection)) return (false, new RestrictionCheckFailedReason.NotInMap());

                    if (_map.IsOccupied(targetCoordPlusDirection)) return (false, new RestrictionCheckFailedReason.NotClearPath());

                    return (true, null);
                }
                case TargetRestriction.Occupied:
                {
                    if (!_map.IsOccupied(targetCoord)) return (false, new RestrictionCheckFailedReason.NotOccupied());
                    return (true, null);
                }
                case TargetRestriction.IsCoordinate:
                {
                    if (target is not Coordinate) return (false, new RestrictionCheckFailedReason.NotCoordinate());

                    return (true, null);
                }
                case TargetRestriction.NotSelf:
                {
                    if (ReferenceEquals(targetEntity, caster)) return (false, new RestrictionCheckFailedReason.NotOthers());

                    return (true, null);
                }

                default:
                    throw new ArgumentOutOfRangeException(nameof(restriction), restriction, null);
            }
        }

        //todo: maybe not nested.
        public abstract record RestrictionCheckFailedReason
        {
            public record TargetTypeNotMatched : RestrictionCheckFailedReason;

            public record NotFriendly : RestrictionCheckFailedReason;

            public record NotEnemy : RestrictionCheckFailedReason;

            public record Occupied : RestrictionCheckFailedReason;

            public record NotClearPath : RestrictionCheckFailedReason;

            public record NotBack : RestrictionCheckFailedReason;

            public record NotSelf : RestrictionCheckFailedReason;

            public record NotOthers : RestrictionCheckFailedReason;

            public record NotObstacle : RestrictionCheckFailedReason;

            public record NotOccupied : RestrictionCheckFailedReason;

            public record NotCoordinate : RestrictionCheckFailedReason;

            public record OutOfRange : RestrictionCheckFailedReason;

            public record CasterNotOnMap : RestrictionCheckFailedReason;

            public record TargetNotOnMap : RestrictionCheckFailedReason;

            public record NotInMap : RestrictionCheckFailedReason;
        }


        #region Target finding

        public IEnumerable<IActionTarget> FindTargets(
            EntityData actor,
            Coordinate targetCoord,
            TargetArea targetArea,
            TargetRestriction restrictionFlags)
        {
            foreach (var coord in GetTargetedCoordinates(actor, targetCoord, targetArea))
            foreach (var target in GetValidTargetsInCoordinate(actor, coord, restrictionFlags))
                yield return target;
        }

        public IEnumerable<Coordinate> GetTargetedCoordinates(
            EntityData actor,
            Coordinate targetCoord,
            TargetArea targetArea)
        {
            foreach (var coordinate in FindUnfilteredCoordinates())
                if (_map.IsCoordinateWithinMap(coordinate))
                    yield return coordinate;

            yield break;

            IEnumerable<Coordinate> FindUnfilteredCoordinates()
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

                        if (!_map.TryFind(actor, out Coordinate actorCoord))
                        {
                            Log.Error("{actor} is not found on the map. We can't figure out the direction of the fan!", actor);
                            yield break;
                        }

                        if (actorCoord.DistanceTo(targetCoord) > 1) Log.Error($"{TargetArea.Fan} cannot deal with anything that's not right next to the actor! This might change later but for now it's unecessarily complicated");

                        var relativeCoord = (targetCoord - actorCoord).Normalized();
                        yield return targetCoord;
                        yield return actorCoord + relativeCoord.RotateLeft();
                        yield return actorCoord + relativeCoord.RotateRight();

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
        }

        private IEnumerable<IActionTarget> GetValidTargetsInCoordinate(
            EntityData caster,
            Coordinate targetCoord,
            TargetRestriction targetRestrictionFlags)
        {
            foreach (var target in GetAllTargets())
            {
                if (CheckRestrictions(target).Any()) continue;

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

            IEnumerable<RestrictionCheckFailedReason> CheckRestrictions(IActionTarget target)
            {
                foreach (var flag in targetRestrictionFlags.GetFlags())
                {
                    var (passedCheck, reason) = CheckTargetRestriction(flag, caster, target);
                    if (!passedCheck) yield return reason!;
                }
            }
        }

        #endregion
    }
}
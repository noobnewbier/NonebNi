using System;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;

namespace NonebNi.Core.Actions
{
    public interface ITargetValidityChecker
    {
        bool IsPassingTargetRestrictionCheck(
            TargetRestriction restriction,
            EntityData caster,
            IActionTarget target);
    }

    public class TargetValidityChecker : ITargetValidityChecker
    {
        private readonly IReadOnlyMap _map;

        public TargetValidityChecker(IReadOnlyMap map)
        {
            _map = map;
        }

        public bool IsPassingTargetRestrictionCheck(
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
using System;
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
            Coordinate targetCoord);
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
            Coordinate targetCoord)
        {
            if (!_map.IsCoordinateWithinMap(targetCoord))
            {
                return false;
            }

            switch (restriction)
            {
                case TargetRestriction.None:
                    return true;
                case TargetRestriction.Friendly:
                {
                    var targetUnit = _map.Get<UnitData>(targetCoord);
                    if (targetUnit == null) return false;

                    return caster.FactionId == targetUnit.FactionId;
                }
                case TargetRestriction.Enemy:
                {
                    var targetUnit = _map.Get<UnitData>(targetCoord);
                    if (targetUnit == null) return false;

                    return caster.FactionId != targetUnit.FactionId;
                }
                case TargetRestriction.NonOccupied:
                {
                    return !_map.Has<EntityData>(targetCoord);
                }
                case TargetRestriction.ClearPath:
                    throw new NotImplementedException("need line algo");
                    break;
                case TargetRestriction.Back:
                    throw new NotImplementedException("back defined by another ally..?");
                    break;
                case TargetRestriction.Self:
                    if (_map.TryFind(caster, out Coordinate casterCoord)) return casterCoord == targetCoord;

                    return false;
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
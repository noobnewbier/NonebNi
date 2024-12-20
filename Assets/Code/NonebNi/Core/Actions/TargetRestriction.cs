using System;

namespace NonebNi.Core.Actions
{
    [Flags]
    public enum TargetRestriction
    {
        None = 0,
        Friendly = 1 << 0,
        Enemy = 1 << 1,
        NonOccupied = 1 << 2,
        ClearPath = 1 << 3,
        Back = 1 << 4,
        Self = 1 << 5,
        Obstacle = 1 << 6,
        FirstTileToTargetDirectionIsEmpty = 1 << 7,
        TargetCoordPlusDirectionToTargetIsEmpty = 1 << 8,
        Occupied = 1 << 9,
        IsCoordinate = 1 << 10,
        NotSelf = 1 << 11
    }
}
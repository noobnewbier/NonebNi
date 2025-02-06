using System.Collections.Generic;
using NonebNi.Core.Coordinates;

namespace NonebNi.Core.Maps
{
    public class HexDirection
    {
        public static readonly HexDirection East = new(new Coordinate(1, 0));
        public static readonly HexDirection West = new(new Coordinate(-1, 0));
        public static readonly HexDirection NorthEast = new(new Coordinate(0, 1));
        public static readonly HexDirection SouthWest = new(new Coordinate(0, -1));
        public static readonly HexDirection SouthEast = new(new Coordinate(1, -1));
        public static readonly HexDirection NorthWest = new(new Coordinate(-1, 1));

        private HexDirection(Coordinate direction)
        {
            Direction = direction;
        }

        private Coordinate Direction { get; }

        public static IEnumerable<HexDirection> All { get; } = new[]
        {
            East,
            West,
            NorthEast,
            SouthWest,
            SouthEast,
            NorthWest
        };

        public static Coordinate operator +(Coordinate a, HexDirection d) => a + d.Direction;

        public static Coordinate operator *(HexDirection d, int i) => d.Direction * i;

        public static Coordinate operator -(Coordinate a, HexDirection d) => a - d.Direction;
    }
}
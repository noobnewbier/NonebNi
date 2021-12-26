using NonebNi.Game.Coordinates;

namespace NonebNi.Game.Maps
{
    public class HexDirection
    {
        public static readonly HexDirection PlusX = new HexDirection(new Coordinate(1, 0));
        public static readonly HexDirection MinusX = new HexDirection(new Coordinate(-1, 0));
        public static readonly HexDirection PlusZ = new HexDirection(new Coordinate(0, 1));
        public static readonly HexDirection MinusZ = new HexDirection(new Coordinate(0, -1));
        public static readonly HexDirection PlusXPlusZ = new HexDirection(new Coordinate(1, 1));
        public static readonly HexDirection MinusXMinusZ = new HexDirection(new Coordinate(-1, -1));

        private Coordinate Direction { get; }

        private HexDirection(Coordinate direction)
        {
            Direction = direction;
        }

        public static Coordinate operator +(Coordinate a, HexDirection d) => a + d.Direction;
    }
}
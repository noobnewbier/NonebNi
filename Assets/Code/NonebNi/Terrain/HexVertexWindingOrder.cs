using System;
using NonebNi.Core.Maps;

namespace NonebNi.Terrain
{
    public static class HexVertexWindingOrder
    {
        private static readonly HexDirection[] DirectionOrder =
        {
            HexDirection.NorthEast,
            HexDirection.East,
            HexDirection.SouthEast,
            HexDirection.SouthWest,
            HexDirection.West,
            HexDirection.NorthWest
        };

        /// <summary>
        /// Get the direction of the next edge representing in the generated mesh in clockwise rotation.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static HexDirection NextDirection(this HexDirection direction)
        {
            var index = Array.IndexOf(DirectionOrder, direction);

            return index == DirectionOrder.Length - 1 ?
                DirectionOrder[0] :
                DirectionOrder[index + 1];
        }

        /// <summary>
        /// Get the direction of the previous edge representing in the generated mesh in clockwise rotation.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static HexDirection PrevDirection(this HexDirection direction)
        {
            var index = Array.IndexOf(DirectionOrder, direction);

            return index == 0 ?
                DirectionOrder[DirectionOrder.Length] :
                DirectionOrder[index - 1];
        }

        public static bool LessThanOrEqualWith(this HexDirection a, HexDirection b)
        {
            return Array.IndexOf(DirectionOrder, a) <= Array.IndexOf(DirectionOrder, b);
        }

        public static bool GreaterThan(this HexDirection a, HexDirection b)
        {
            return Array.IndexOf(DirectionOrder, a) > Array.IndexOf(DirectionOrder, b);
        }

        public static int GetVerticesWindingOrderIndex(this HexDirection direction)
        {
            return Array.IndexOf(DirectionOrder, direction);
        }
    }
}
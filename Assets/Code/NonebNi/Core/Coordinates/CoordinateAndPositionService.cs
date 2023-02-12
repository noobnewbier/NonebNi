using NonebNi.Core.Level;
using UnityEngine;

namespace NonebNi.Core.Coordinates
{
    public interface ICoordinateAndPositionService
    {
        Vector3 FindPosition(Coordinate coordinate);
        Coordinate NearestCoordinateForPoint(Vector3 point);

        /// <summary>
        /// Find out if a position resides within a given coordinate.
        /// </summary>
        /// <param name="point">The position to test</param>
        /// <param name="coordinate">Coordinate to test</param>
        /// <returns>Whether the <see cref="point" /> is within the given <see cref="coordinate" /></returns>
        bool IsPointWithinCoordinate(Vector3 point, Coordinate coordinate);
    }

    /// <summary>
    /// Given the <see cref="WorldConfigData" />, convert between coordinate and position
    /// </summary>
    public class CoordinateAndPositionService : ICoordinateAndPositionService
    {
        private readonly WorldConfigData _worldConfig;

        public CoordinateAndPositionService(WorldConfigData worldConfig)
        {
            _worldConfig = worldConfig;
        }

        private float SideDistanceOfHex => _worldConfig.InnerRadius * 2f;

        private float UpDistanceOfHex => _worldConfig.OuterRadius * 1.5f;

        public Vector3 FindPosition(Coordinate coordinate)
        {
            float FindSideOffsetForZ(int zIndex)
            {
                var sideDistance = SideDistanceOfHex;

                // ReSharper disable once PossibleLossOfFraction - that's intentional. We need the floored whole number here
                return zIndex % 2 * sideDistance / 2f + zIndex / 2 * sideDistance;
            }

            var sideOffset = FindSideOffsetForZ(coordinate.Z);

            return new Vector3(
                       coordinate.X * SideDistanceOfHex + sideOffset,
                       _worldConfig.MapStartingPosition.y,
                       coordinate.Z * UpDistanceOfHex
                   ) +
                   _worldConfig.MapStartingPosition;
        }

        public Coordinate NearestCoordinateForPoint(Vector3 point)
        {
            /*
             * Convert the position into a fractional coordinate, round it to the nearest hex.
             * Reference: https://www.redblobgames.com/grids/hexagons/#rounding
             */
            var x = (Mathf.Sqrt(3) / 3 * point.x - 1f / 3 * point.z) / _worldConfig.OuterRadius;
            var z = 2f / 3 * point.z / _worldConfig.OuterRadius;
            var y = -x - z;

            //temporarily working on cube coordinate here
            var roundX = Mathf.RoundToInt(x);
            var roundY = Mathf.RoundToInt(y);
            var roundZ = Mathf.RoundToInt(z);

            var xDiff = Mathf.Abs(x - roundX);
            var yDiff = Mathf.Abs(y - roundY);
            var zDiff = Mathf.Abs(z - roundZ);

            var maxDiff = Mathf.Max(xDiff, yDiff, zDiff);
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (maxDiff == xDiff) roundX = -roundY - roundZ;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            else if (maxDiff == zDiff) roundZ = -roundX - roundY;

            return new Coordinate(roundX, roundZ);
        }

        /// <summary>
        /// Find out if a position resides within a given coordinate.
        /// </summary>
        /// <param name="point">The position to test</param>
        /// <param name="coordinate">Coordinate to test</param>
        /// <returns>Whether the <see cref="point" /> is within the given <see cref="coordinate" /></returns>
        public bool IsPointWithinCoordinate(Vector3 point, Coordinate coordinate) =>
            NearestCoordinateForPoint(point) == coordinate;
    }
}
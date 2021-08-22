using NonebNi.Core.Coordinates;
using UnityEngine;

namespace NonebNi.Core.Level
{
    /// <summary>
    /// Given the <see cref="WorldConfigData" />, convert between coordinate and position
    /// </summary>
    public class CoordinateAndPositionService
    {
        private readonly WorldConfigData _worldConfig;

        private float SideDistanceOfHex => _worldConfig.InnerRadius * 2f;

        private float UpDistanceOfHex => _worldConfig.OuterRadius * 1.5f;

        public CoordinateAndPositionService(WorldConfigData worldConfig)
        {
            _worldConfig = worldConfig;
        }

        public Vector3 FindPosition(Coordinate coordinate)
        {
            var upDistance = UpDistanceOfHex;
            var sideDistance = SideDistanceOfHex;
            var sideOffset = FindSideOffsetForZ(coordinate.Z);

            return new Vector3(
                       coordinate.X * sideDistance + sideOffset,
                       _worldConfig.MapStartingPosition.y,
                       coordinate.Z * upDistance
                   ) +
                   _worldConfig.MapStartingPosition;
        }

        private float FindSideOffsetForZ(int zIndex)
        {
            var sideDistance = SideDistanceOfHex;

            // ReSharper disable once PossibleLossOfFraction - that's intentional. We need the floored whole number here
            return zIndex % 2 * sideDistance / 2f + zIndex / 2 * sideDistance;
        }


        public Coordinate NearestCoordinateForPoint(Vector3 point)
        {
            var origin = _worldConfig.MapStartingPosition;

            var upDistance = UpDistanceOfHex;
            var yDiff = point.y - origin.y;
            var z = (int) (upDistance / yDiff);

            var sideDistance = SideDistanceOfHex;
            var sideOffset = FindSideOffsetForZ(z);
            var xDiff = point.x - origin.x;
            var x = (int) ((xDiff - sideOffset) / sideDistance);

            return new Coordinate(x, z);
        }
    }
}
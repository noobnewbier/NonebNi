using NonebNi.Core.Coordinates;
using UnityEngine;
using UnityUtils;

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

            var zDiff = point.z - origin.z;
            var z = (int) (zDiff / UpDistanceOfHex);

            var sideOffset = FindSideOffsetForZ(z);
            var xDiff = point.x - origin.x;
            var x = (int) ((xDiff - sideOffset) / SideDistanceOfHex);

            return new Coordinate(x, z);
        }

        /// <summary>
        /// Find out if a position resides within a given coordinate in the current level
        /// </summary>
        /// <param name="point">The position to test</param>
        /// <param name="coordinate">Coordinate to test</param>
        /// <returns>Whether the <see cref="point" /> is within the given <see cref="coordinate" /></returns>
        public bool IsPointWithinCoordinate(Vector3 point, Coordinate coordinate)
        {
            /*
             * This works by drawing a line for each side of the regular hexagon.
             * For each line, we find out if the given position and the center of the coordinate is on the same side of the line.
             *
             * If the above check is true for all six lines. We know that the given position is within the given coordinate
             */
            var coordinatePos = FindPosition(coordinate);
            var outerRadius = _worldConfig.OuterRadius;

            const float thirtyDegToRad = 30f * Mathf.Deg2Rad;
            var tan30 = Mathf.Tan(thirtyDegToRad);

            //note as we are thinking in XZ plane, z is used in place of y.
            var pointPos2D = (point - coordinatePos).XZ();

            var topRightLine = new LinearEquations.LinearEquation2D(-tan30, 1, +outerRadius);
            var rightLine = new LinearEquations.LinearEquation2D(1, 0, +outerRadius);
            var bottomRightLine = new LinearEquations.LinearEquation2D(tan30, 1, -outerRadius);
            var bottomLeftLine = new LinearEquations.LinearEquation2D(-tan30, 1, -outerRadius);
            var leftLine = new LinearEquations.LinearEquation2D(1, 0, -outerRadius);
            var topLeftLine = new LinearEquations.LinearEquation2D(tan30, 1, +outerRadius);

            var pointTopRightSign = (int) Mathf.Sign(topRightLine.Solve(pointPos2D));
            var pointRightSign = (int) Mathf.Sign(rightLine.Solve(pointPos2D));
            var pointBottomRightLineSign = (int) Mathf.Sign(bottomRightLine.Solve(pointPos2D));
            var pointBottomLeftLineSign = (int) Mathf.Sign(bottomLeftLine.Solve(pointPos2D));
            var pointLeftLineSign = (int) Mathf.Sign(leftLine.Solve(pointPos2D));
            var pointTopLeftLineSign = (int) Mathf.Sign(topLeftLine.Solve(pointPos2D));

            //There are no need to calculate where the center resides relative to the line - it is always the same
            const int centerTopRightSign = -1;
            const int centerRightSign = -1;
            const int centerBottomRightLineSign = 1;
            const int centerBottomLeftLineSign = 1;
            const int centerLeftLineSign = 1;
            const int centerTopLeftLineSign = -1;

            return centerTopRightSign == pointTopRightSign &&
                   centerRightSign == pointRightSign &&
                   centerBottomRightLineSign == pointBottomRightLineSign &&
                   centerBottomLeftLineSign == pointBottomLeftLineSign &&
                   centerLeftLineSign == pointLeftLineSign &&
                   centerTopLeftLineSign == pointTopLeftLineSign;
        }
    }
}
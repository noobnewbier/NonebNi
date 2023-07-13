using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using UnityEngine;

namespace NonebNi.Terrain
{
    public interface ICoordinateAndPositionService
    {
        IEnumerable<Vector3> FindCorners(Coordinate coordinate);
        Vector3 FindPosition(Coordinate coordinate);
        Coordinate NearestCoordinateForPoint(Vector3 point);

        /// <summary>
        ///     Find out if a position resides within a given coordinate.
        /// </summary>
        /// <param name="point">The position to test</param>
        /// <param name="coordinate">Coordinate to test</param>
        /// <returns>Whether the <see cref="point" /> is within the given <see cref="coordinate" /></returns>
        bool IsPointWithinCoordinate(Vector3 point, Coordinate coordinate);

        /// <summary>
        ///     Get the first outer cell corner for a direction.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="direction">The desired direction.</param>
        /// <returns>The corner on the counter-clockwise side.</returns>
        Vector3 GetFirstSolidCorner(Coordinate coordinate, HexDirection direction);

        /// <summary>
        ///     Get the second outer cell corner for a direction.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="direction">The desired direction.</param>
        /// <returns>The corner on the clockwise side.</returns>
        Vector3 GetSecondSolidCorner(Coordinate coordinate, HexDirection direction);

        Vector3 GetBridge(Coordinate coordinate, HexDirection direction);
        Vector3 GetBridge(HexDirection direction);
    }

    /// <summary>
    ///     Given the <see cref="TerrainConfigData" />, convert between coordinate and position
    /// </summary>
    public class CoordinateAndPositionService : ICoordinateAndPositionService
    {
        /// <summary>
        ///     Factor of the solid uniform region inside a hex cell.
        /// </summary>
        public const float SolidFactor = 0.8f;

        /// <summary>
        ///     Factor of the blending region inside a hex cell.
        /// </summary>
        public const float BlendFactor = 1f - SolidFactor;

        private readonly TerrainConfigData _terrainConfig;


        //Origin from center, begin from top, rotate clockwise
        private readonly Vector3[] _tileCornersOffset;

        public CoordinateAndPositionService(TerrainConfigData terrainConfig)
        {
            _terrainConfig = terrainConfig;
            _tileCornersOffset = new[]
            {
                new Vector3(0f, 0f, _terrainConfig.OuterRadius),
                new Vector3(_terrainConfig.InnerRadius, 0f, 0.5f * _terrainConfig.OuterRadius),
                new Vector3(_terrainConfig.InnerRadius, 0f, -0.5f * _terrainConfig.OuterRadius),
                new Vector3(0f, 0f, -_terrainConfig.OuterRadius),
                new Vector3(-_terrainConfig.InnerRadius, 0f, -0.5f * _terrainConfig.OuterRadius),
                new Vector3(-_terrainConfig.InnerRadius, 0f, 0.5f * _terrainConfig.OuterRadius)
            };
        }

        private float SideDistanceOfHex => _terrainConfig.InnerRadius * 2f;

        private float UpDistanceOfHex => _terrainConfig.OuterRadius * 1.5f;

        public IEnumerable<Vector3> FindCorners(Coordinate coordinate)
        {
            var center = FindPosition(coordinate);
            var corners = _tileCornersOffset.Select(c => center + c);

            return corners;
        }

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
                       _terrainConfig.MapStartingPosition.y,
                       coordinate.Z * UpDistanceOfHex
                   ) +
                   _terrainConfig.MapStartingPosition;
        }

        public Coordinate NearestCoordinateForPoint(Vector3 point)
        {
            /*
             * Convert the position into a fractional coordinate, round it to the nearest hex.
             * Reference: https://www.redblobgames.com/grids/hexagons/#rounding
             */
            var x = (Mathf.Sqrt(3) / 3 * point.x - 1f / 3 * point.z) / _terrainConfig.OuterRadius;
            var z = 2f / 3 * point.z / _terrainConfig.OuterRadius;
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
        ///     Find out if a position resides within a given coordinate.
        /// </summary>
        /// <param name="point">The position to test</param>
        /// <param name="coordinate">Coordinate to test</param>
        /// <returns>Whether the <see cref="point" /> is within the given <see cref="coordinate" /></returns>
        public bool IsPointWithinCoordinate(Vector3 point, Coordinate coordinate) =>
            NearestCoordinateForPoint(point) == coordinate;

        public Vector3 GetBridge(Coordinate coordinate, HexDirection direction)
        {
            var coordinatePos = FindPosition(coordinate);

            return GetBridge(direction) + coordinatePos;
        }

        public Vector3 GetBridge(HexDirection direction)
        {
            var firstCorner = _tileCornersOffset[direction.GetVerticesWindingOrderIndex()];
            var secondCorner = _tileCornersOffset[direction.NextDirection().GetVerticesWindingOrderIndex()];

            return (firstCorner + secondCorner) * BlendFactor;
        }

        /// <summary>
        ///     Get the first outer cell corner for a direction.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="direction">The desired direction.</param>
        /// <returns>The corner on the counter-clockwise side.</returns>
        public Vector3 GetFirstSolidCorner(Coordinate coordinate, HexDirection direction)
        {
            var coordinatePos = FindPosition(coordinate);

            return coordinatePos + _tileCornersOffset[direction.GetVerticesWindingOrderIndex()] * SolidFactor;
        }

        /// <summary>
        ///     Get the second outer cell corner for a direction.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="direction">The desired direction.</param>
        /// <returns>The corner on the clockwise side.</returns>
        public Vector3 GetSecondSolidCorner(Coordinate coordinate, HexDirection direction)
        {
            var coordinatePos = FindPosition(coordinate);

            return coordinatePos +
                   _tileCornersOffset[direction.NextDirection().GetVerticesWindingOrderIndex()] * SolidFactor;
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace NonebNi.Art.GrassInstancing
{
    // The algorithm is from the "Fast Poisson Disk Sampling in Arbitrary Dimensions" paper by Robert Bridson.
    // https://www.cs.ubc.ca/~rbridson/docs/bridson-siggraph07-poissondisk.pdf

    public static class BlueNoise
    {
        public const float InvertRootTwo = 0.70710678118f; // Becaust two dimension grid.
        public const int DefaultIterationPerPoint = 30;


        public static IEnumerable<Vector3> Sampling(Vector3 bottomLeft, Vector3 topRight, float minimumDistance) => Sampling(bottomLeft, topRight, minimumDistance, DefaultIterationPerPoint);

        public static IEnumerable<Vector3> Sampling(Vector3 bottomLeft, Vector3 topRight, float minimumDistance, int iterationPerPoint)
        {
            var settings = GetSettings
            (
                bottomLeft,
                topRight,
                minimumDistance,
                iterationPerPoint <= 0 ?
                    DefaultIterationPerPoint :
                    iterationPerPoint
            );

            var bags = new Bags
            {
                Grid = new Vector3?[settings.GridWidth + 1, settings.GridHeight + 1, settings.GridDepth + 1],
                ActivePoints = new ()
            };

            yield return GetFirstPoint(settings, bags);

            do
            {
                var index = Random.Range(0, bags.ActivePoints.Count);

                var point = bags.ActivePoints[index];

                bool anyFound = false;
                for (var k = 0; k < settings.IterationPerPoint; k++)
                {
                    var samplePoint = GetNextPoint(point, settings, bags);
                    anyFound |= samplePoint.HasValue;
                    if (samplePoint.HasValue)
                    {
                        yield return samplePoint.Value;
                    }
                }

                if (!anyFound) bags.ActivePoints.RemoveAt(index);
            } while (bags.ActivePoints.Count > 0);
        }

        #region "Structures"

        private class Settings
        {
            public Vector3 BottomLeft;

            public float CellSize;
            public Bounds Dimension;
            public int GridDepth;
            public int GridHeight;
            public int GridWidth;
            public int IterationPerPoint;

            public float MinimumDistance;
            public Vector3 TopRight;
        }

        private class Bags
        {
            public List<Vector3> ActivePoints = new ();
            public Vector3?[,,] Grid;
        }

        #endregion

        #region "Algorithm Calculations"

        private static Vector3? GetNextPoint(Vector3 point, Settings set, Bags bags)
        {
            var ptInSphere = Random.onUnitSphere * Random.Range(set.MinimumDistance, 2f * set.MinimumDistance);

            var p = new Vector3(ptInSphere.x, 0, ptInSphere.z) + point;

            if (!set.Dimension.Contains(p)) return null;

            var minimum = set.MinimumDistance * set.MinimumDistance;
            var index = GetGridIndex(p, set);
            var drop = false;

            var around = 2;
            var fieldMin = new Vector3Int(Mathf.Max(0, index.x - around), Mathf.Max(0, index.y - around), Mathf.Max(0, index.z - around));
            var fieldMax = new Vector3Int(Mathf.Min(set.GridWidth, index.x + around), Mathf.Min(set.GridHeight, index.y + around), Mathf.Min(set.GridDepth, index.z + around));

            for (var i = fieldMin.x; i <= fieldMax.x && !drop; i++)
            for (var j = fieldMin.y; j <= fieldMax.y && !drop; j++)
            for (var k = fieldMin.z; k <= fieldMax.z && !drop; k++)
            {
                var q = bags.Grid[i, j, k];
                if (q.HasValue && (q.Value - p).sqrMagnitude <= minimum) drop = true;
            }

            if (!drop)
            {
                bags.ActivePoints.Add(p);
                bags.Grid[index.x, index.y, index.z] = p;
                return p;
            }

            return null;
        }

        private static Vector3 GetFirstPoint(Settings set, Bags bags)
        {
            var first = new Vector3(Random.Range(set.BottomLeft.x, set.TopRight.x), Random.Range(set.BottomLeft.y, set.TopRight.y), Random.Range(set.BottomLeft.z, set.TopRight.z));

            var index = GetGridIndex(first, set);

            bags.Grid[index.x, index.y, index.z] = first;
            bags.ActivePoints.Add(first);
            return first;
        }

        #endregion

        #region "Utils"

        private static Vector3Int GetGridIndex(Vector3 point, Settings set) => new (Mathf.FloorToInt((point.x - set.BottomLeft.x) / set.CellSize), Mathf.FloorToInt((point.y - set.BottomLeft.y) / set.CellSize), Mathf.FloorToInt((point.z - set.BottomLeft.z) / set.CellSize));

        private static Settings GetSettings(Vector3 bl, Vector3 tr, float min, int iteration)
        {
            var dimension = tr - bl;
            var cell = min * InvertRootTwo;
            var bd = new Bounds
            {
                min = bl,
                max = tr
            };
            return new ()
            {
                BottomLeft = bl,
                TopRight = tr,
                Dimension = bd,

                MinimumDistance = min,
                IterationPerPoint = iteration,

                CellSize = cell,
                GridWidth = Mathf.CeilToInt(dimension.x / cell),
                GridHeight = Mathf.CeilToInt(dimension.y / cell),
                GridDepth = Mathf.CeilToInt(dimension.z / cell)
            };
        }

        #endregion
    }
}
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;
using UnityEngine;
using UnityUtils;

namespace NonebNi.Core.Pathfinding
{
    public interface IPathfindingService
    {
        (bool isPathExist, IEnumerable<Coordinate> path) FindPath(EntityData entity, Coordinate goal);
        (bool isPathExist, IEnumerable<Coordinate> path) FindPath(UnitData unit, Coordinate goal);
        (bool isPathExist, IEnumerable<Coordinate> path) FindPath(Coordinate start, Coordinate goal);
    }

    public class PathfindingService : IPathfindingService
    {
        private readonly IReadOnlyMap _map;

        public PathfindingService(IReadOnlyMap map)
        {
            _map = map;
        }

        public (bool isPathExist, IEnumerable<Coordinate> path) FindPath(EntityData entity, Coordinate goal)
        {
            var isOnMap = _map.TryFind(entity, out Coordinate entityPos);
            if (!isOnMap)
            {
                Debug.LogWarning(
                    "Trying to find a path from an entity that doesn't exist on the map - something went wrong?"
                );
                return (false, Enumerable.Empty<Coordinate>());
            }

            var (isPathExist, path) = FindPath(entityPos, goal);
            var pathAsArray = path as Coordinate[] ?? path.ToArray();

            return (isPathExist, pathAsArray);
        }

        public (bool isPathExist, IEnumerable<Coordinate> path) FindPath(UnitData unit, Coordinate goal)
        {
            var (isPathExist, path) = FindPath(unit as EntityData, goal);
            if (!isPathExist) return (false, Enumerable.Empty<Coordinate>());

            var pathAsArray = path as Coordinate[] ?? path.ToArray();
            if (pathAsArray.Length > unit.Speed) return (false, Enumerable.Empty<Coordinate>());

            return (isPathExist, pathAsArray);
        }

        // ReSharper disable once CognitiveComplexity - it's just an A* implementation we copy from wiki, no need to fix.
        public (bool isPathExist, IEnumerable<Coordinate> path) FindPath(Coordinate start, Coordinate goal)
        {
            /*
             * If performance became an issue:
             *  1. We can start using a priority queue.
             *  2. Early exit when we reached a max distance(e.g. unit's max moving distance)
             *
             * But for now we should be good.
             *
             * ref: https://en.wikipedia.org/wiki/A*_search_algorithm
             */

            //No path exists if the goal coordinate is already occupied.
            if (_map.IsOccupied(goal)) return (false, Enumerable.Empty<Coordinate>());

            var openSet = new HashSet<Coordinate>
            {
                start
            };
            var cameFrom = new Dictionary<Coordinate, Coordinate>();
            var gScore = new DefaultDictionary<Coordinate, float>(TileData.ObstacleWeight)
            {
                [start] = 0
            };

            var fScore = new DefaultDictionary<Coordinate, float>(TileData.ObstacleWeight)
            {
                [start] = Heuristic(start, goal)
            };

            while (openSet.Any())
            {
                var current = openSet.MinBy(c => fScore[c]);
                openSet.Remove(current);

                if (current == goal)
                {
                    IEnumerable<Coordinate> ReconstructPath()
                    {
                        do
                        {
                            yield return current;
                            current = cameFrom[current];
                        } while (cameFrom.ContainsKey(current));
                    }

                    return (true, ReconstructPath());
                }

                foreach (var neighbour in current.Neighbours)
                {
                    if (!_map.TryGet(neighbour, out TileData? neighbourTileData))
                        //Doesn't exist in the map
                        continue;

                    var tentativeGScore = gScore[current] + neighbourTileData.Value.Weight;
                    if (tentativeGScore >= gScore[neighbour]) continue;

                    cameFrom[neighbour] = current;
                    gScore[neighbour] = tentativeGScore;
                    fScore[neighbour] = tentativeGScore + Heuristic(neighbour, goal);

                    openSet.Add(neighbour);
                }
            }

            return (false, Enumerable.Empty<Coordinate>());
        }

        private static float Heuristic(Coordinate tileCoordinate, Coordinate goalCoordinate)
        {
            var diff = tileCoordinate - goalCoordinate;

            return (Mathf.Abs(diff.X) + Mathf.Abs(diff.Y) + Mathf.Abs(diff.Z)) / 2f;
        }

        /// <summary>
        ///     Analogue of Python's default dict - just make our implementation slightly cleaner
        /// </summary>
        private class DefaultDictionary<TKey, TValue> : Dictionary<TKey, TValue>
        {
            private readonly TValue _defaultValue;

            public DefaultDictionary(TValue defaultValue)
            {
                _defaultValue = defaultValue;
            }

            public new TValue this[TKey key]
            {
                get
                {
                    if (!TryGetValue(key, out var val))
                    {
                        val = _defaultValue;
                        Add(key, val);
                    }

                    return val;
                }
                set => base[key] = value;
            }
        }
    }
}
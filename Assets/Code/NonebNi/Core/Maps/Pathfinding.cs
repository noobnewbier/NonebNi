﻿using System.Collections.Generic;
using System.Linq;
using Noneb.Core.Game.Coordinates;
using Noneb.Core.Game.Tiles;
using Priority_Queue;
using UnityEngine;

namespace Noneb.Core.Game.Maps
{
    //https://en.wikipedia.org/wiki/A*_search_algorithm
    public static class Pathfinding
    {
        //There is no need to aggressively early exit for now, we are in no need for such optimization
        public static bool TryFindPath(Coordinate start,
                                       Coordinate goal,
                                       Map map,
                                       out IList<Coordinate> path,
                                       int maxCost = int.MaxValue,
                                       bool includeStartingTile = false)
        {
            var tileToDiscover = new SimplePriorityQueue<Coordinate, float>();
            var cameFrom = new Dictionary<Coordinate, Coordinate>();
            var distanceToTile = new Dictionary<Coordinate, float> {[start] = 0f};

            tileToDiscover.Enqueue(start, Heuristic(start, goal));
            path = null;

            while (tileToDiscover.Any())
            {
                var current = tileToDiscover.Dequeue();
                if (current == goal)
                {
                    path = new List<Coordinate>();
                    while (current != start)
                    {
                        path.Add(current);
                        current = cameFrom[current];
                    }

                    //we ignore the starting tile when calculating the cost
                    var pathCost = path.Select(map.Get<Tile>).Sum(t => t.Data.Weight);
                    if (includeStartingTile) path.Add(start);

                    path = path.Reverse().ToList();

                    return pathCost <= maxCost;
                }

                foreach (var neighbour in map.GetNeighbours<Tile>(current).Values)
                {
                    if (neighbour == null)
                        //ignore tiles that does not exist(e.g. when current is at the top/bottom edge of the map)
                        continue;

                    var currentDistanceToNeighbour = distanceToTile[current] + neighbour.Data.Weight;
                    var neighbourCoordinate = neighbour.Coordinate;
                    if (!distanceToTile.TryGetValue(neighbourCoordinate, out var previousDistanceToNeighbour))
                        previousDistanceToNeighbour = float.PositiveInfinity;

                    if (currentDistanceToNeighbour > previousDistanceToNeighbour) continue;

                    var newScoreForNeighbour = currentDistanceToNeighbour + Heuristic(neighbourCoordinate, goal);
                    cameFrom[neighbourCoordinate] = current;
                    distanceToTile[neighbourCoordinate] = currentDistanceToNeighbour;
                    if (!tileToDiscover.TryUpdatePriority(neighbourCoordinate, newScoreForNeighbour))
                        tileToDiscover.Enqueue(neighbourCoordinate, newScoreForNeighbour);
                }
            }

            return false;
        }

        private static float Heuristic(Coordinate tileCoordinate, Coordinate goalCoordinate) =>
            Mathf.Abs(goalCoordinate.X - tileCoordinate.X) +
            Mathf.Abs(goalCoordinate.Y - tileCoordinate.Y) +
            Mathf.Abs(goalCoordinate.Z - tileCoordinate.Z);
    }
}
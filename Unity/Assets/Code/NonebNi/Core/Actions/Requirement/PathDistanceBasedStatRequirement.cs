using System;
using System.Linq;
using Noneb.Logs.Runtime;
using NonebNi.Core.Commands;
using NonebNi.Core.Coordinates;
using NonebNi.Core.GameContexts;
using NonebNi.Core.Maps;
using NonebNi.Core.Pathfinding;
using NonebNi.Core.Stats;
using UnityEngine;

namespace NonebNi.Core.Actions
{
    [Serializable]
    public class PathDistanceBasedStatRequirement : StatRequirement
    {
        [SerializeField] private StatCost costPerHex;

        public PathDistanceBasedStatRequirement(StatCost costPerHex)
        {
            this.costPerHex = costPerHex;
        }

        public override StatCost CalculateCost(ActionCommand command, IReadOnlyMap map)
        {
            var deps = GameContext.GetImmediate<Dependencies>();
            if (command.TargetCoords.Count == 0)
            {
                Log.Warn("Action", "No target coords found. There's no range!");
                return costPerHex * 0;
            }

            if (!map.TryFind(command.ActorEntity, out Coordinate actorCoord))
            {
                Log.Error("Action", $"Couldn't find actor: {command.ActorEntity}, how can I work with range without the actor on the map?");
                return costPerHex * 0;
            }

            if (command.TargetCoords.Count > 1)
            {
                Log.Warn("Action", "Not sure how to deal with situations where you have more than one target coordinates, I am just gonna take the first one.");
            }

            var targetCoord = command.TargetCoords.First();
            var (success, path) = deps.PathfindingService.FindPath(actorCoord, targetCoord);
            if (!success)
            {
                // some result that the player won't be able to pay.
                return costPerHex * 9999;
            }

            return costPerHex * path.Count();
        }

        public record Dependencies(IPathfindingService PathfindingService);
    }
}
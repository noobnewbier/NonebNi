using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Decisions;
using NonebNi.Core.Diagnostics;
using NonebNi.Core.Effects;
using NonebNi.Core.Factions;
using NonebNi.Core.GameContexts;
using NonebNi.Core.Maps;
using NonebNi.Core.Units;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityUtils;
using Action = Unity.Behavior.Action;

namespace NonebNi.Core.AI
{
    //todo: debug flag to fake damage.
    //todo: debug command to force certain action...? can we create tree in runtime to facilitate this?
    //todo: wbn: if we have a way to visualize the utility based on positioning/distance. So yes, yet another curve.
    //todo: give attributes
    [Serializable, GeneratePropertyBag, NodeDescription("EngageNode", story: "Engage Enemy to [preferredDistance]", category: "Action", id: "ab7d8a170a42a51524ad6ddfe5760658")]
    public partial class EngageNode : Action, IUtilityNode //todo: define melee distance. there's no notion of melee in the action itself.
    {
        [SerializeReference] public BlackboardVariable<int> preferredDistance = new ()
        {
            Value = 1
        };

        public async IAsyncEnumerable<(ActionCommand command, Utility utility)> FindActionAndUtility(UnitData controlledUnit)
        {
            /*
             * - Max score for position that allow for melee attack
             * - Otherwise higher score for any node that is closer to position that can attack enemy
             * - Doesn't take distance into account - it only cares if we are approaching enemy, if you want to take distance into account, use in combination of other nodes....?
             */

            var deps = await GameContext.Get<Dependencies>();

            var agentContext = this.GetContext();
            if (agentContext == null) yield break;

            var actions = controlledUnit.Actions;
            var enemyCoords = deps.Map.GetAllUnits()
                                  .Where(u => !deps.FactionService.IsAlly(controlledUnit.FactionId, u.FactionId))
                                  .Select(u => deps.Map.Find(u))
                                  .ToArray();

            // if we are already in that distance, although there might be a better position else where, we don't do anything for now
            var currentCoord = deps.Map.Find(controlledUnit);
            var currentDistToEnemies = enemyCoords.Select(c => currentCoord.DistanceTo(c)).Min();
            if (currentDistToEnemies == preferredDistance.Value) yield break;

            var commandDistances = FindCommandAndDiffWithTargetDistance(controlledUnit, currentCoord, enemyCoords, actions, deps);
            if (!commandDistances.Any()) yield break;

            var minDistDiff = commandDistances.Values.Min(t => t.distToTarget);
            var maxDistDiff = commandDistances.Values.Max(t => t.distToTarget);
            foreach (var (command, result) in commandDistances)
            {
                var targetCoord = command.TargetCoords.First();

                // distance is the most important metric
                var distFromSelf = currentCoord.DistanceTo(targetCoord);
                var distancePenalty = distFromSelf * 0.001f; //minor penalty to prioritize closest tile
                var distScore = 1 - Mathf.InverseLerp(minDistDiff, maxDistDiff, result.distToTarget) - distancePenalty;

                // prefer coords that is on the direct line to the target, helps to avoid units running into a separate "lane"
                var outOfLinePenalty = 0f;
                if (!currentCoord.GetCoordinatesBetween(result.enemyCoordTarget).Contains(result.enemyCoordTarget)) outOfLinePenalty = 0.001f;

                // prefer line that is more zigzaggy, as it implies there's less lane crossing
                var zigzagness = currentCoord.ZigZagnessWithinRange(targetCoord);
                var crossLanePenalty = (1 - zigzagness) * 0.001f;

                var score = distScore - outOfLinePenalty - crossLanePenalty;
                yield return (command, (UtilityTag.DistanceEngagement, score));

                Diagnostic.Write(new DRequest.EngageUtility(targetCoord, distScore, outOfLinePenalty, crossLanePenalty));
            }
        }

        private Dictionary<ActionCommand, (Coordinate enemyCoordTarget, int distToTarget)> FindCommandAndDiffWithTargetDistance(UnitData controlledUnit, Coordinate unitCoord, Coordinate[] enemyCoords, NonebAction[] actions, Dependencies deps)
        {
            var actionDistanceDiff = new Dictionary<ActionCommand, (Coordinate enemyCoordTarget, int distToTarget)>();

            // prioritize going to the closest bunch.
            enemyCoords = enemyCoords.GroupBy(unitCoord.DistanceTo).MinBy(g => g.Key).ToArray();

            foreach (var action in actions)
            {
                //ignoring MoveOver/Swap for now - those are a bit too complicated and unnecessary for prototype 
                var moveEffect = action.Effects.OfType<MoveEffect>().FirstOrDefault();

                if (moveEffect == null) continue;

                foreach (var command in deps.OptionFinder.FindOptionsForActor(controlledUnit).OfType<ActionCommand>())
                {
                    //This doesn't care about obstacles atm, feels off... We will add the complexity when we need to.
                    var targetCoord = command.TargetCoords.First(); // can't work without this, if we don't have one something else is buggered.
                    var (enemyCoordTarget, distToTarget) = enemyCoords.Select(c => (enemyCoordTarget: c, distToTarget: targetCoord.DistanceTo(c))).MinBy(t => t.distToTarget);
                    var diffToPreferredDist = Mathf.Abs(preferredDistance.Value - distToTarget);
                    actionDistanceDiff[command] = (enemyCoordTarget, diffToPreferredDist);
                }
            }

            return actionDistanceDiff;
        }

        public record Dependencies(IReadOnlyMap Map, IActionOptionFinder OptionFinder, IFactionService FactionService);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Decisions;
using NonebNi.Core.Effects;
using NonebNi.Core.Factions;
using NonebNi.Core.GameContexts;
using NonebNi.Core.Maps;
using NonebNi.Core.Units;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

namespace NonebNi.Core.AI
{
    //todo: we have the basics now - enemy should come and try attack me. turns out this is not happening yay
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

        //todo: act -> if enemy in range -> engage, else find heat and approach heat.
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

            /*
             * find all the possible movement, use the one that matches the preferred distance the most - doesn't care if we have more than one enemy for now
             * we can add possibility using influence map later
             */
            var commandDistances = FindCommandAndDiffWithTargetDistance(controlledUnit, enemyCoords, actions, deps);
            if (!commandDistances.Any()) yield break;

            var minDistDiff = commandDistances.Values.Min();
            var maxDistDiff = commandDistances.Values.Max();
            foreach (var (command, distDiff) in commandDistances)
            {
                var score = 1 - Mathf.InverseLerp(minDistDiff, maxDistDiff, distDiff);
                yield return (command, (UtilityTag.DistanceEngagement, score)); //todo: sth is wrong here.
            }
        }

        private Dictionary<ActionCommand, int> FindCommandAndDiffWithTargetDistance(UnitData controlledUnit, Coordinate[] enemyCoords, NonebAction[] actions, Dependencies deps)
        {
            var actionDistanceDiff = new Dictionary<ActionCommand, int>();

            foreach (var action in actions)
            {
                //ignoring MoveOver/Swap for now - those are a bit too complicated and unnecessary for prototype 
                var moveEffect = action.Effects.OfType<MoveEffect>().FirstOrDefault();

                if (moveEffect == null) continue;

                foreach (var command in deps.OptionFinder.FindOptionsForActor(controlledUnit).OfType<ActionCommand>())
                {
                    //This doesn't care about obstacles atm, feels off... We will add the complexity when we need to.
                    var targetCoord = command.TargetCoords.First(); // can't work without this, if we don't have one something else is buggered.
                    var dist = enemyCoords.Select(c => targetCoord.DistanceTo(c)).Min();
                    actionDistanceDiff[command] = Mathf.Abs(preferredDistance.Value - dist);
                }
            }

            return actionDistanceDiff;
        }

        public record Dependencies(IReadOnlyMap Map, IActionOptionFinder OptionFinder, IFactionService FactionService);
    }
}
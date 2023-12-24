using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using Unity.Logging;
using UnityUtils;

namespace NonebNi.Core.Commands.Handlers
{
    public class ActionCommandHandler : ICommandHandler<ActionCommand>
    {
        private readonly IMap _map;
        private readonly ITargetValidityChecker _targetValidityChecker;

        public ActionCommandHandler(IMap map, ITargetValidityChecker targetValidityChecker)
        {
            _map = map;
            _targetValidityChecker = targetValidityChecker;
        }

        public IEnumerable<ISequence> Evaluate(ActionCommand command)
        {
            return command.Action.Effects.SelectMany(
                e =>
                {
                    var targets = FindTargets(
                        command.ActorEntity,
                        command.TargetCoord,
                        command.Action.TargetArea,
                        command.Action.TargetRestriction
                    );
                    return e.Evaluate(_map, command.ActorEntity, targets);
                }
            );
        }

        private IEnumerable<IActionTarget> FindTargets(
            EntityData actor,
            Coordinate targetCoord,
            TargetArea targetArea,
            TargetRestriction restriction)
        {
            foreach (var coord in GetTargetedCoordinates(actor, targetCoord, targetArea))
            foreach (var target in GetValidTargetsInCoordinate(actor, coord, restriction))
                yield return target;
        }

        private IEnumerable<Coordinate> GetTargetedCoordinates(
            EntityData actor,
            Coordinate targetCoord,
            TargetArea targetArea)
        {
            switch (targetArea)
            {
                case TargetArea.Single:
                    yield return targetCoord;
                    break;
                case TargetArea.Fan:
                    if (actor.IsSystem)
                    {
                        Log.Error($"{TargetArea.Fan} is not supported for System Entity");
                        yield break;
                    }

                    if (_map.TryFind(actor, out Coordinate actorCoord))
                    {
                        Log.Error("Actor is not found on the map. We can't figure out the direction of the fan!");
                        yield break;
                    }

                    var relativeCoord = targetCoord - actorCoord;
                    yield return targetCoord;
                    yield return relativeCoord.RotateLeft();
                    yield return relativeCoord.RotateRight();

                    break;
                case TargetArea.Circle:
                    foreach (var neighbour in targetCoord.Neighbours)
                        if (_map.IsCoordinateWithinMap(neighbour))
                            yield return neighbour;

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(targetArea), targetArea, null);
            }
        }

        private IEnumerable<IActionTarget> GetValidTargetsInCoordinate(
            EntityData caster,
            Coordinate targetCoord,
            TargetRestriction targetRestrictionFlags)
        {
            foreach (var target in GetAllTargets())
            {
                if (FailedAnyTargetRestrictionCheck(target)) continue;

                yield return target;
            }

            yield break;

            IEnumerable<IActionTarget> GetAllTargets()
            {
                if (_map.TryGet(targetCoord, out IEnumerable<EntityData>? entities))
                {
                    //Stuffs move around when effect is evaluated, this leads to changes in the entities
                    //so we make a copy so we aren't modifying the enumeration in a foreach loop
                    var targetsCopy = entities.ToArray();
                    foreach (var entity in targetsCopy)
                        yield return entity;
                }

                yield return targetCoord;
            }

            bool FailedAnyTargetRestrictionCheck(IActionTarget target)
            {
                return targetRestrictionFlags.GetFlags().Any(f => FailedTargetRestrictionCheck(f, target));
            }

            bool FailedTargetRestrictionCheck(TargetRestriction restriction, IActionTarget target)
            {
                return !_targetValidityChecker.IsPassingTargetRestrictionCheck(
                    restriction,
                    caster,
                    target
                );
            }
        }
    }
}
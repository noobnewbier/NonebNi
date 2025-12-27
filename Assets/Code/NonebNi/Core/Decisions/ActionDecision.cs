using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;

namespace NonebNi.Core.Decisions
{
    public record ActionDecision : IDecision
    {
        public readonly NonebAction Action;
        public readonly EntityData ActorEntity;
        public readonly Coordinate[] TargetCoords;

        public ActionDecision(NonebAction action, EntityData actorEntity, IEnumerable<Coordinate> targetCoords) : this(action, actorEntity, targetCoords.ToArray()) { }

        public ActionDecision(ActionCommand command) : this(command.Action, command.ActorEntity, command.TargetCoords) { }

        public ActionDecision(NonebAction action, EntityData actorEntity, params Coordinate[] targetCoords)
        {
            Action = action;
            ActorEntity = actorEntity;
            TargetCoords = targetCoords;
        }

        public virtual bool Equals(ActionDecision? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Action.Equals(other.Action) && ActorEntity.Equals(other.ActorEntity) && TargetCoords.SequenceEqual(other.TargetCoords);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();

            hash.Add(Action);
            hash.Add(ActorEntity);

            foreach (var coord in TargetCoords) hash.Add(coord);

            return hash.ToHashCode();
        }
    }
}
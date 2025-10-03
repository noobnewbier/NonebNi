using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;

namespace NonebNi.Core.Commands
{
    public record ActionCommand : ICommand
    {
        public readonly NonebAction Action;

        public readonly EntityData ActorEntity;

        public readonly IReadOnlyList<Coordinate> TargetCoords;

        public ActionCommand(NonebAction action, EntityData actorEntity, params Coordinate[] targetCoords)
        {
            Action = action;
            ActorEntity = actorEntity;
            TargetCoords = targetCoords;
        }

        public virtual bool Equals(ActionCommand? other)
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
using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Decisions;
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

        public ActionCommand(ActionDecision decision) : this(decision.Action, decision.ActorEntity, decision.TargetCoords) { }

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

        /// <summary>
        /// Decision is just a command that might not be valid gameplay logic wise(player might not be able to afford the cost,
        /// etc)
        /// Most of the time, the two are interchangeable, so here we go.
        /// I might regret later
        /// </summary>
        public static implicit operator ActionCommand(ActionDecision decision) => new (decision);
    }
}
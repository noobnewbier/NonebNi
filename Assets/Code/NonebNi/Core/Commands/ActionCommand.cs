using NonebNi.Core.Actions;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;

namespace NonebNi.Core.Commands
{
    public class ActionCommand : ICommand
    {
        public readonly Action Action;
        public readonly EntityData ActorEntity;
        public readonly Coordinate TargetCoord;

        public ActionCommand(Action action, EntityData actorEntity, Coordinate targetCoord)
        {
            Action = action;
            ActorEntity = actorEntity;
            TargetCoord = targetCoord;
        }
    }
}
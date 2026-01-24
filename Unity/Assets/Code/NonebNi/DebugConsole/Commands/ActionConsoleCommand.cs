using System.Text;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using NonebNi.Core.Commands;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Units;
using NonebNi.DebugConsole.Commands;
using NonebNi.DebugConsole.Commands.Attributes;

namespace NonebNi.DebugConsole.Commands
{
    [Command("action", "cast an action from the actor to the target tile, bypassing decision validation")]
    [UsedImplicitly]
    public class ActionConsoleCommand : IConsoleCommand
    {
        public readonly string ActionId;
        public readonly Coordinate? ActorCoord;
        public readonly Coordinate[] TargetCoords;

        public ActionConsoleCommand(
            [CommandParam("the ID of the casted action")] string actionId,
            [CommandParam("coordinate of the target")] Coordinate targetCoord)
        {
            ActorCoord = null;
            TargetCoords = new[] { targetCoord };
            ActionId = actionId;
        }

        public ActionConsoleCommand(
            [CommandParam("the ID of the casted action")] string actionId,
            [CommandParam("coordinate of the target")] params Coordinate[] targetCoords)
        {
            ActorCoord = null;
            TargetCoords = targetCoords;
            ActionId = actionId;
        }

        public ActionConsoleCommand(
            [CommandParam("the ID of the casted action")] string actionId,
            [CommandParam("coordinate of the actor, if left empty, we will use the current unit to perform the action")] Coordinate actorCoord,
            [CommandParam("coordinate of the target")] params Coordinate[] targetCoords)
        {
            ActorCoord = actorCoord;
            TargetCoords = targetCoords;
            ActionId = actionId;
        }
    }
}

namespace NonebNi.DebugConsole
{
    public partial class CommandHandler
    {
        private UniTask DoHandle(ActionConsoleCommand command, StringBuilder outputBuffer)
        {
            var action = _actionRepository.GetAction(command.ActionId);
            if (action == null)
            {
                outputBuffer.AppendLine(
                    $"Unable to find action with matching action ID: {command.ActionId}"
                );
                return UniTask.CompletedTask;
            }

            UnitData? unit;
            if (command.ActorCoord == null)
                unit = _turnOrderer.CurrentUnit;
            else if (!_readOnlyMap.TryGet(command.ActorCoord, out unit)) return UniTask.CompletedTask;

            EvaluateSequence(new ActionCommand(action, unit, command.TargetCoords));

            return UniTask.CompletedTask;
        }
    }
}
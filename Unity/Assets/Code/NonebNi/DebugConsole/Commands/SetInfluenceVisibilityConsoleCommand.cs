using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using NonebNi.Core.DataIds;
using NonebNi.Core.Factions;
using NonebNi.DebugConsole.Commands;
using NonebNi.DebugConsole.Commands.Attributes;

namespace NonebNi.DebugConsole.Commands
{
    [Command(
         "show-influence",
         "Set visibility influence of factions"
     ), UsedImplicitly]
    public class SetInfluenceVisibilityConsoleCommand : IConsoleCommand
    {
        public readonly string[] FactionIds;
        public readonly bool IsVisible;

        public SetInfluenceVisibilityConsoleCommand(
            [CommandParam("is visible or not")] bool isVisible,
            [CommandParam("List of faction to show")] string[] factionIds)
        {
            IsVisible = isVisible;
            FactionIds = factionIds;
        }

        /*
         * Note
         * It would be nicer to:
         * 1. Change the lexer so it can deal with array params as the last parameter without the angular bracket, which works better in practice
         * 2. Change the expression parser so these can be DataId<Faction> instead
         *
         * That said, both of these are going to take time and I am not sure if it's necessary, so here we go.
         */
        public SetInfluenceVisibilityConsoleCommand(
            [CommandParam("is visible or not")] bool isVisible,
            [CommandParam("First faction to show")] string faction1,
            [CommandParam("Second faction to show")] string faction2,
            [CommandParam("Third faction to show")] string faction3)
        {
            IsVisible = isVisible;
            FactionIds = new[] { faction1, faction2, faction3 };
        }

        public SetInfluenceVisibilityConsoleCommand(
            [CommandParam("is visible or not")] bool isVisible,
            [CommandParam("First faction to show")] string faction1,
            [CommandParam("Second faction to show")] string faction2)
        {
            IsVisible = isVisible;
            FactionIds = new[] { faction1, faction2 };
        }

        public SetInfluenceVisibilityConsoleCommand(
            [CommandParam("is visible or not")] bool isVisible,
            [CommandParam("First faction to show")] string faction1)
        {
            IsVisible = isVisible;
            FactionIds = new[] { faction1 };
        }
    }
}

namespace NonebNi.DebugConsole
{
    public partial class CommandHandler
    {
        private UniTask DoHandle(SetInfluenceVisibilityConsoleCommand command, StringBuilder _)
        {
            _influenceMapVisualizer.SetEnable(command.IsVisible, command.FactionIds.Select(i => new DataId<Faction>(i)));
            return UniTask.CompletedTask;
        }
    }
}
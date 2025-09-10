using JetBrains.Annotations;
using NonebNi.DebugConsole.Commands.Attributes;

namespace NonebNi.DebugConsole.Commands
{
    [Command("endTurn", "inflicts set amount of damage to unit on coordinate")]
    [UsedImplicitly]
    public class EndTurnDecisionCommand : IConsoleCommand { }
}
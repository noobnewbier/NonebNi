using JetBrains.Annotations;
using NonebNi.Core.Coordinates;

namespace NonebNi.EditorConsole.Commands
{
    [Command("damage", "inflicts set amount of damage to unit on coordinate")]
    [UsedImplicitly]
    public class DamageConsoleCommand : IConsoleCommand
    {
        public readonly Coordinate Coordinate;
        public readonly int Damage;

        public DamageConsoleCommand(
            [CommandParam("target coordinate, any unit on coordinate will take the specified damage")]
            Coordinate coordinate,
            [CommandParam("amount of damage that will be dealt to unit on the target coordinate")]
            int damage)
        {
            Coordinate = coordinate;
            Damage = damage;
        }
    }
}
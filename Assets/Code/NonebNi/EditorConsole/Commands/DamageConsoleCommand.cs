using NonebNi.Core.Coordinates;

namespace NonebNi.EditorConsole.Commands
{
    public class DamageConsoleCommand : IConsoleCommand
    {
        public readonly Coordinate Coordinate;
        public readonly int Damage;

        public DamageConsoleCommand(Coordinate coordinate, int damage)
        {
            Coordinate = coordinate;
            Damage = damage;
        }
    }
}
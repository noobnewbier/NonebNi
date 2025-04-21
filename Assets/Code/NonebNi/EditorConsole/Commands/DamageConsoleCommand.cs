using JetBrains.Annotations;
using NonebNi.Core.Actions;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Effects;
using NonebNi.EditorConsole.Commands.Attributes;

namespace NonebNi.EditorConsole.Commands
{
    [Command("damage", "inflicts set amount of damage to unit on coordinate")]
    [UsedImplicitly]
    public class DamageConsoleCommand : IConsoleActionCommand
    {
        private readonly int _damage;
        public readonly Coordinate Coordinate;

        public DamageConsoleCommand(
            [CommandParam("target coordinate, any unit on coordinate will take the specified damage")] Coordinate coordinate,
            [CommandParam("amount of damage that will be dealt to unit on the target coordinate")] int damage)
        {
            Coordinate = coordinate;
            _damage = damage;
        }

        public NonebAction GetAction()
        {
            return new NonebAction(
                "debug-damage",
                0,
                0,
                10000,
                TargetArea.Single,
                TargetRestriction.None,
                false,
                new DamageEffect("slash", _damage)
            );
        }
    }
}
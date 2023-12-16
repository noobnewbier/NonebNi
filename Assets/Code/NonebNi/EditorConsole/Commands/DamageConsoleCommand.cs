using JetBrains.Annotations;
using NonebNi.Core.Actions;
using NonebNi.Core.Actions.Effects;
using NonebNi.Core.Coordinates;
using NonebNi.EditorConsole.Commands.Attributes;

namespace NonebNi.EditorConsole.Commands
{
    [Command("damage", "inflicts set amount of damage to unit on coordinate")]
    [UsedImplicitly]
    public class DamageConsoleCommand : IConsoleActionCommand
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

        public NonebAction GetAction()
        {
            return new NonebAction(
                "debug-damage",
                10000,
                TargetRestriction.None,
                TargetArea.Single,
                "none",
                0,
                new Effect[] { new DamageEffect(Damage) }
            );
        }
    }
}
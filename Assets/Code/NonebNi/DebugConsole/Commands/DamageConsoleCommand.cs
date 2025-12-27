using System.Text;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Effects;
using NonebNi.Core.Entities;
using NonebNi.Core.Units;
using NonebNi.DebugConsole.Commands;
using NonebNi.DebugConsole.Commands.Attributes;

namespace NonebNi.DebugConsole.Commands
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
                string.Empty,
                new DamageEffect("slash", _damage)
            );
        }
    }
}

namespace NonebNi.DebugConsole
{
    public partial class CommandHandler
    {
        private UniTask DoHandle(DamageConsoleCommand command, StringBuilder _)
        {
            if (_readOnlyMap.TryGet<UnitData>(command.Coordinate, out var __))
                EvaluateSequence(
                    new ActionCommand(
                        command.GetAction(),
                        SystemEntity.Instance,
                        command.Coordinate
                    )
                );

            return UniTask.CompletedTask;
        }
    }
}
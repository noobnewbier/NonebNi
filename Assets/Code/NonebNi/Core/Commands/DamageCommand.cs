using System.Collections.Generic;
using NonebNi.Core.Units;

namespace NonebNi.Core.Commands
{
    public class DamageCommand : ICommand
    {
        public readonly int Damage;
        public readonly IEnumerable<UnitData> Targets;

        public DamageCommand(int damage, params UnitData[] targets)
        {
            Damage = damage;
            Targets = targets;
        }

        public DamageCommand(int damage, IEnumerable<UnitData> targets)
        {
            Damage = damage;
            Targets = targets;
        }
    }
}
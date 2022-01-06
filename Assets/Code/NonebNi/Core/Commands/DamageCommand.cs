using System.Collections.Generic;
using NonebNi.Core.Units;

namespace NonebNi.Core.Commands
{
    public class DamageCommand : ICommand
    {
        private readonly int _damage;
        private readonly IEnumerable<UnitData> _targets;

        public DamageCommand(int damage, IEnumerable<UnitData> targets)
        {
            _damage = damage;
            _targets = targets;
        }

        public IEnumerable<UnitData> Evaluate()
        {
            foreach (var target in _targets) target.Health -= _damage;

            return _targets;
        }
    }
}
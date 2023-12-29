using NonebNi.Core.Effects;

namespace NonebNi.Core.Actions
{
    //TODO: this is temporary before we figure out how do we store action/effects, most likely through SO w/ potential to transition to Json 
    public static class ActionDatas
    {
        private static readonly Range weaponBasedRange = new WeaponBasedRange();

        public static readonly NonebAction Move = new(
            "move",
            new StatBasedRange(1f, StatBasedRange.StatType.Speed),
            new[] { TargetRestriction.NonOccupied },
            TargetArea.Single,
            0,
            new Effect[] { new MoveEffect() }
        );

        public static readonly NonebAction Bash = new(
            "bash",
            1,
            new[] { TargetRestriction.Enemy },
            TargetArea.Single,
            1,
            new Effect[] { new KnockBackEffect(1) }
        );

        public static readonly NonebAction Shoot = new(
            "shoot",
            weaponBasedRange,
            new[] { TargetRestriction.Enemy },
            TargetArea.Single,
            1,
            new Effect[] { new DamageEffect(new StatBasedDamage(1f, StatBasedDamage.StatType.Focus)) }
        );

        public static readonly NonebAction PowerShot = new(
            "power-shot",
            weaponBasedRange,
            new[] { TargetRestriction.Enemy | TargetRestriction.ClearPath },
            TargetArea.Single,
            1,
            new Effect[]
            {
                new DamageEffect(new StatBasedDamage(1f, StatBasedDamage.StatType.Focus)),
                new KnockBackEffect(1)
            }
        );

        public static readonly NonebAction Strike = new(
            "strike",
            1,
            new[] { TargetRestriction.Enemy },
            TargetArea.Single,
            1,
            new Effect[]
            {
                new DamageEffect(new StatBasedDamage(1f, StatBasedDamage.StatType.Strength))
            }
        );

        public static readonly NonebAction Swing = new(
            "swing",
            1,
            new[] { TargetRestriction.Enemy },
            TargetArea.Fan,
            1,
            new Effect[]
            {
                new DamageEffect(new StatBasedDamage(1f, StatBasedDamage.StatType.Strength)),
                new KnockBackEffect(1)
            }
        );

        public static readonly NonebAction Slash = new(
            "slash",
            1,
            new[] { TargetRestriction.Enemy },
            TargetArea.Single,
            1,
            new Effect[]
            {
                new DamageEffect(new StatBasedDamage(1f, StatBasedDamage.StatType.Focus))
            }
        );

        public static readonly NonebAction TacticalAdvance = new(
            "tactical-advance",
            new StatBasedRange(1, StatBasedRange.StatType.Focus),
            new[] { TargetRestriction.Friendly, TargetRestriction.NonOccupied },
            TargetArea.Single,
            1,
            new Effect[] { new MoveEntityEffect() }
        );

        public static NonebAction[] Actions =
        {
            Move,
            Bash,
            Shoot,
            PowerShot,
            Strike,
            Swing,
            Slash,
            TacticalAdvance
        };
    }
}
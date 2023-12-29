using NonebNi.Core.Effects;

namespace NonebNi.Core.Actions
{
    //TODO: this is temporary before we figure out how do we store action/effects, most likely through SO w/ potential to transition to Json 
    public static class ActionDatas
    {
        private static readonly Range WeaponBasedRange = new WeaponBasedRange();

        public static readonly NonebAction Move = new(
            "move",
            new StatBasedRange(1f, StatBasedRange.StatType.Speed),
            TargetRestriction.NonOccupied,
            TargetArea.Single,
            0,
            new MoveEffect()
        );

        public static readonly NonebAction Bash = new(
            "bash",
            1,
            TargetRestriction.Enemy,
            TargetArea.Single,
            1,
            new KnockBackEffect(1)
        );

        public static readonly NonebAction Shoot = new(
            "shoot",
            WeaponBasedRange,
            TargetRestriction.Enemy,
            TargetArea.Single,
            1,
            new DamageEffect(new StatBasedDamage(1f, StatBasedDamage.StatType.Focus))
        );

        public static readonly NonebAction PowerShot = new(
            "power-shot",
            WeaponBasedRange,
            TargetRestriction.Enemy | TargetRestriction.ClearPath,
            TargetArea.Single,
            1,
            new DamageEffect(new StatBasedDamage(1f, StatBasedDamage.StatType.Focus)),
            new KnockBackEffect(1)
        );

        public static readonly NonebAction Strike = new(
            "strike",
            1,
            TargetRestriction.Enemy,
            TargetArea.Single,
            1,
            new DamageEffect(new StatBasedDamage(1f, StatBasedDamage.StatType.Strength))
        );

        public static readonly NonebAction Swing = new(
            "swing",
            1,
            TargetRestriction.Enemy,
            TargetArea.Fan,
            1,
            new DamageEffect(new StatBasedDamage(1f, StatBasedDamage.StatType.Strength)),
            new KnockBackEffect(1)
        );

        public static readonly NonebAction Slash = new(
            "slash",
            1,
            TargetRestriction.Enemy,
            TargetArea.Single,
            1,
            new DamageEffect(new StatBasedDamage(1f, StatBasedDamage.StatType.Focus))
        );

        public static readonly NonebAction TacticalAdvance = new(
            "tactical-advance",
            new StatBasedRange(1, StatBasedRange.StatType.Focus),
            new[]
            {
                TargetRestriction.Friendly,
                TargetRestriction.NonOccupied
            },
            TargetArea.Single,
            1,
            new MoveEntityEffect()
        );

        public static readonly NonebAction Lure = new(
            "lure",
            new StatBasedRange(1, StatBasedRange.StatType.Focus),
            TargetRestriction.Enemy | TargetRestriction.ClearPath | TargetRestriction.FirstTileToTargetDirectionIsEmpty,
            TargetArea.Single,
            1,
            new PullEntityEffect()
        );
        
        public static readonly NonebAction Vault = new(
            "vault",
            1,
            new[]
            {
                TargetRestriction.Occupied | TargetRestriction.TargetCoordPlusDirectionToTargetIsEmpty |
                TargetRestriction.IsCoordinate
            },
            TargetArea.Single,
            1,
            new MoveOverEffect()
        );

        public static readonly NonebAction Grapple = new(
            "grapple",
            new Range[] { 2, 1 },
            new[]
            {
                TargetRestriction.Enemy,
                TargetRestriction.NonOccupied
            },
            TargetArea.Single,
            1,
            new MoveEntityEffect()
        );

        public static readonly NonebAction Vault = new(
            "vault",
            1,
            new[]
            {
                TargetRestriction.Occupied | TargetRestriction.TargetCoordPlusDirectionToTargetIsEmpty |
                TargetRestriction.IsCoordinate
            },
            TargetArea.Single,
            1,
            new MoveOverEffect()
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
            TacticalAdvance,
            Lure,
            Grapple,
            Vault
        };
    }
}
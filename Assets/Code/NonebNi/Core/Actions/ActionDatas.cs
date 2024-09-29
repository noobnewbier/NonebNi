using NonebNi.Core.Effects;
using static NonebNi.Core.Actions.TargetArea;
using static NonebNi.Core.Actions.TargetRestriction;

namespace NonebNi.Core.Actions
{
    //TODO: this is temporary before we figure out how do we store action/effects, most likely through SO w/ potential to transition to Json 
    public static class ActionDatas
    {
        private static readonly Range WeaponBasedRange = new WeaponBasedRange();

        public static readonly NonebAction Move = new(
            "move",
            new StatBasedRange(1f, StatBasedRange.StatType.Speed),
            NonOccupied,
            Single,
            0,
            new MoveEffect()
        );

        public static readonly NonebAction Bash = new(
            "bash",
            1,
            Enemy,
            Single,
            1,
            new KnockBackEffect(1)
        );

        public static readonly NonebAction Shoot = new(
            "shoot",
            WeaponBasedRange,
            Enemy,
            Single,
            1,
            new DamageEffect("shoot", new StatBasedDamage(1f, StatBasedDamage.StatType.Focus))
        );

        public static readonly NonebAction PowerShot = new(
            "power-shot",
            WeaponBasedRange,
            Enemy | ClearPath,
            Single,
            1,
            new DamageEffect("power-shot", new StatBasedDamage(1f, StatBasedDamage.StatType.Focus)),
            new KnockBackEffect(1)
        );

        public static readonly NonebAction Strike = new(
            "strike",
            1,
            Enemy,
            Single,
            1,
            new DamageEffect("strike", new StatBasedDamage(1f, StatBasedDamage.StatType.Strength))
        );

        public static readonly NonebAction Swing = new(
            "swing",
            1,
            Enemy,
            Fan,
            1,
            new DamageEffect("swing", new StatBasedDamage(1f, StatBasedDamage.StatType.Strength)),
            new KnockBackEffect(1)
        );

        public static readonly NonebAction Slash = new(
            "slash",
            1,
            Enemy,
            Single,
            1,
            new DamageEffect("slash", new StatBasedDamage(1f, StatBasedDamage.StatType.Focus))
        );

        public static readonly NonebAction TacticalAdvance = new(
            "tactical-advance",
            new StatBasedRange(1, StatBasedRange.StatType.Focus),
            new[]
            {
                Friendly,
                NonOccupied
            },
            Single,
            1,
            new MoveEntityEffect()
        );

        public static readonly NonebAction Lure = new(
            "lure",
            new StatBasedRange(1, StatBasedRange.StatType.Focus),
            Enemy | ClearPath | FirstTileToTargetDirectionIsEmpty,
            Single,
            1,
            new PullEntityEffect()
        );

        public static readonly NonebAction Vault = new(
            "vault",
            1,
            Occupied | TargetCoordPlusDirectionToTargetIsEmpty | IsCoordinate,
            Single,
            1,
            new MoveOverEffect()
        );

        public static readonly NonebAction Grapple = new(
            "grapple",
            new Range[] { 2, 1 },
            new[]
            {
                Enemy,
                NonOccupied
            },
            Single,
            1,
            new MoveEntityEffect()
        );

        public static readonly NonebAction Rotate = new(
            "rotate",
            1,
            Friendly | NotSelf,
            Single,
            1,
            new SwapPositionEffect()
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
            Vault,
            Rotate
        };
    }
}
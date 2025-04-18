using System.Linq;
using NonebNi.Core.Effects;
using UnityEngine;
using static NonebNi.Core.Actions.TargetArea;
using static NonebNi.Core.Actions.TargetRestriction;

namespace NonebNi.Core.Actions
{
    /*
     * TODO:
     * Future self - sort out the animation, find a way to test it, and move on.
     * The animation doesn't need to look great, you need a proof of concept now,
     * try to find a way such that the "being hit animation" and the "hit people animation" is playing in tandem, then it's good enough.
     *
     * ## Current Goal
     * Fit following animation into both tank and fighter:
     * 1. strike
     * 2. receive damage
     * 3. move
     */


    //TODO: this is temporary before we figure out how do we store action/effects, most likely through SO w/ potential to transition to Json
    //TODO: there's no way we can collect all(mod included) dependencies at compile, as some of them don't even exist, we will need to collect them via some form of reflection...? 
    public static class ActionDatas
    {
        //TODO: remember - "likely" all actions, including the ones from the base game should go into the mods. i don't really want to make exceptions for stuffs like moving(base-gameplay)/debugging(utilities tool)...
        private static readonly Range WeaponBasedRange = new WeaponBasedRange();

        public static readonly NonebAction Move = new(
            "move",
            "move",
            Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
            0,
            1,
            new StatBasedRange(1f, StatBasedRange.StatType.Speed),
            Single,
            NonOccupied,
            new MoveEffect()
        );

        public static readonly NonebAction Bash = new(
            "bash",
            "bash",
            Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
            1,
            1,
            1,
            Single,
            Enemy,
            new KnockBackEffect(1)
        );

        public static readonly NonebAction Shoot = new(
            "shoot",
            "shoot",
            Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
            1,
            1,
            WeaponBasedRange,
            Single,
            Enemy,
            new DamageEffect("shoot", new StatBasedDamage(1f, StatBasedDamage.StatType.Focus))
        );

        public static readonly NonebAction PowerShot = new(
            "power-shot",
            "power-shot",
            Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
            1,
            1,
            WeaponBasedRange,
            Single,
            Enemy | ClearPath,
            new DamageEffect("power-shot", new StatBasedDamage(1f, StatBasedDamage.StatType.Focus)),
            new KnockBackEffect(1)
        );

        public static readonly NonebAction Strike = new(
            "strike",
            "strike",
            Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
            1,
            1,
            1,
            Single,
            Enemy,
            new DamageEffect("strike", new StatBasedDamage(1f, StatBasedDamage.StatType.Strength))
        );

        public static readonly NonebAction Swing = new(
            "swing",
            "swing",
            Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
            1,
            1,
            1,
            Fan,
            Enemy,
            new DamageEffect("swing", new StatBasedDamage(1f, StatBasedDamage.StatType.Strength)),
            new KnockBackEffect(1)
        );

        public static readonly NonebAction Slash = new(
            "slash",
            "slash",
            Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
            1,
            1,
            1,
            Single,
            Enemy,
            new DamageEffect("slash", new StatBasedDamage(1f, StatBasedDamage.StatType.Focus))
        );

        public static readonly NonebAction TacticalAdvance = new(
            "tactical-advance",
            "tactical-advance",
            Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
            1,
            1,
            new StatBasedRange(1, StatBasedRange.StatType.Focus),
            Single,
            new[] { Friendly, NonOccupied },
            new MoveEntityEffect()
        );

        public static readonly NonebAction Lure = new(
            "lure",
            "lure",
            Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
            1,
            1,
            new StatBasedRange(1, StatBasedRange.StatType.Focus),
            Single,
            Enemy | ClearPath | FirstTileToTargetDirectionIsEmpty,
            new PullEntityEffect()
        );

        public static readonly NonebAction Vault = new(
            "vault",
            "vault",
            Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
            1,
            1,
            1,
            Single,
            Occupied | TargetCoordPlusDirectionToTargetIsEmpty | IsCoordinate,
            new MoveOverEffect()
        );

        public static readonly NonebAction Grapple = new(
            "grapple",
            "grapple",
            Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
            1,
            1,
            new[]
            {
                new TargetRequest(Enemy, Single, 2),
                new TargetRequest(Enemy, Single, 1)
            },
            new MoveEntityEffect()
        );

        public static readonly NonebAction Rotate = new(
            "rotate",
            "rotate",
            Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
            1,
            1,
            1,
            Single,
            Friendly | NotSelf,
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

        public static NonebAction? Find(string id)
        {
            return Actions.FirstOrDefault(a => a.Id == id);
        }
    }
}
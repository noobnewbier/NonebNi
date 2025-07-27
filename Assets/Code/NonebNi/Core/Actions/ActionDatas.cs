using System.Linq;
using NonebNi.Core.Effects;
using NonebNi.Core.Stats;
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

    //todo: make sure, all actions, fucking work, including combos
    //todo: ai after that should be "trivial"
    //todo: ui seems bugged

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
            new[]
            {
                new StatCost(StatId.Speed, 1)
            },
            new[]
            {
                new TargetRequest(NonOccupied, Single, new StatBasedRange(1f, StatId.Speed))
            },
            new Effect[]
            {
                new MoveEffect()
            },
            false,
            "Moving to target position"
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
            true,
            "Use your shield to knock back enemy for one tile, does little damage",
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
            false,
            "Shoot an enemy at range, does good damage",
            new DamageEffect("shoot", new StatBasedDamage(1f, StatId.Focus))
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
            true,
            "Shoot at a clear target on a straight line, the arrow is so powerful it knockbacks the target by one tile",
            new DamageEffect("power-shot", new StatBasedDamage(1f, StatId.Focus)),
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
            false,
            "An overhead strike that does ok damage",
            new DamageEffect("strike", new StatBasedDamage(1f, StatId.Strength))
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
            true,
            "Swinging your zweihander to intimidate your enemy, does okay damage and knock back nearby enemies by one tile",
            new DamageEffect("swing", new StatBasedDamage(1f, StatId.Strength)),
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
            false,
            "A quick slash with your short sword, dealing okay damage",
            new DamageEffect("slash", new StatBasedDamage(1f, StatId.Focus))
        );

        public static readonly NonebAction TacticalAdvance = new(
            "tactical-advance",
            "tactical-advance",
            Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
            1,
            1,
            new StatBasedRange(1, StatId.Focus),
            Single,
            new[] { Friendly, NonOccupied },
            true,
            "Commanding another friendly unit to move to another position",
            new MoveEntityEffect() //todo: path finding, and change how the range works?
        );

        //todo: clear path might not be working as intended. time to whip out our automatic test before it's too late.
        public static readonly NonebAction Lure = new(
            "lure",
            "lure",
            Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
            1,
            1,
            new StatBasedRange(1, StatId.Focus),
            Single,
            Enemy | ClearPath | FirstTileToTargetDirectionIsEmpty,
            true,
            "Faking an opening and draw enemy towards you",
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
            false,
            "Jumping over an obstacle and land in front of it",
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
                new TargetRequest(NonOccupied, Single, 1)
            },
            true,
            "Grabbing an enemy to a near by position",
            new MoveEntityEffect()
        ); // todo: doesn't work.

        public static readonly NonebAction Rotate = new(
            "rotate",
            "rotate",
            Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
            1,
            1,
            1,
            Single,
            Friendly | NotSelf,
            true,
            "Using your footwork to swap position with an ally",
            new SwapPositionEffect()
        ); //todo: doesn't work

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
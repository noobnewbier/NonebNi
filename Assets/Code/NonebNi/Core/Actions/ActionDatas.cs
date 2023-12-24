using NonebNi.Core.Effects;

namespace NonebNi.Core.Actions
{
    //TODO: this is temporary before we figure out how do we store action/effects, most likely through SO w/ potential to transition to Json 
    public static class ActionDatas
    {
        public static readonly NonebAction Move = new(
            "move",
            1,
            TargetRestriction.NonOccupied,
            TargetArea.Single,
            0,
            new Effect[] { new MoveEffect() }
        );

        public static readonly NonebAction Bash = new(
            "bash",
            1,
            TargetRestriction.Enemy,
            TargetArea.Single,
            1,
            new Effect[] { new KnockBackEffect(1) }
        );

        public static NonebAction[] Actions =
        {
            Move,
            Bash
        };
    }
}
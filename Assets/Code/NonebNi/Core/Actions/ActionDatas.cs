using NonebNi.Core.Effects;

namespace NonebNi.Core.Actions
{
    //TODO: this is temporary before we figure out how do we store action/effects, most likely through SO w/ potential to transition to Json 
    public static class ActionDatas
    {
        public static readonly NonebAction MoveAction = new(
            "move",
            1,
            TargetRestriction.NonOccupied,
            TargetArea.Single,
            "idk just a place holder", //TODO: idk just a place holder
            0,
            new Effect[] { new MoveEffect() }
        );

        public static NonebAction[] Actions =
        {
            MoveAction,
            new(
                "bash",
                1,
                TargetRestriction.Enemy,
                TargetArea.Single,
                "idk just another place holder",
                1,
                new Effect[] { new KnockBackEffect(1) }
            )
        };
    }
}
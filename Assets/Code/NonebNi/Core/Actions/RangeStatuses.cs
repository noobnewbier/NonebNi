using System.Collections.Generic;

namespace NonebNi.Core.Actions
{
    //TODO: as we work on ui these will be more defined, for now this can be wishy-washy/not having any idea what these do is okay. we will regret later
    public abstract record RangeStatus
    {
        public record Targetable : RangeStatus;

        public record OutOfRange : RangeStatus;

        public record InRangeButNoTarget : RangeStatus;

        public record NotTargetable(IReadOnlyList<TargetFinder.RestrictionCheckFailedReason> Reasons) : RangeStatus;
    }
}
using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.Maps;

namespace NonebNi.Core.Decisions
{
    /// <summary>
    ///     Takes a <see cref="IDecision" /> and check if it's valid, if it is turns it into a <see cref="ICommand" />.
    ///     Difference between a command and a decision is that a decision can be invalid gameplay-wise,
    ///     this is because it's something (potentially) coming from a player and might not be valid at all.
    ///     In contrast all command, unless systematically impossible(multiple unit on the same spot, moving unit that doesn't
    ///     exist),
    ///     it will always happen, regardless if the command abides by the gameplay rule.
    /// </summary>
    public interface IDecisionValidator
    {
        (Error? error, ICommand command) ValidateDecision(IDecision? decision);

        /// <summary>
        ///     Describe why a decision is invalid.
        /// </summary>
        public class Error
        {
            public const string UnknownId = "unknown";

            public Error(string id, string description)
            {
                Id = id;
                Description = description;
            }

            public string Id { get; }
            public string Description { get; }

            public static Error Unknown { get; } = new(UnknownId, "Failed for an undefined reason");
        }
    }

    public class DecisionValidator : IDecisionValidator
    {
        private readonly IReadOnlyMap _map; //TODO: PathFinding
        private readonly ITargetValidityChecker _targetValidityChecker;

        public DecisionValidator(IReadOnlyMap map, ITargetValidityChecker targetValidityChecker)
        {
            _map = map;
            _targetValidityChecker = targetValidityChecker;
        }

        public (IDecisionValidator.Error? error, ICommand command) ValidateDecision(IDecision? decision)
        {
            switch (decision)
            {
                case EndTurnDecision:
                    return (null, new EndTurnCommand());
                case ActionDecision ad:
                    if (!_targetValidityChecker.IsPassingTargetRestrictionCheck(
                            ad.Action.TargetRestriction,
                            ad.ActorEntity,
                            ad.TargetCoord
                        ))
                    {
                        return (
                            new IDecisionValidator.Error(
                                "invalid-target",
                                $"action {ad.Action} cannot be targeted at {ad.TargetCoord}"
                            ), new ActionCommand(ad.Action, ad.ActorEntity, ad.TargetCoord)); //TODO: validate    
                    }

                    return (null, new ActionCommand(ad.Action, ad.ActorEntity, ad.TargetCoord));
                default:
                    return (IDecisionValidator.Error.Unknown, NullCommand.Instance);
            }
        }
    }
}
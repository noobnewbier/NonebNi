namespace NonebNi.Core.Decisions
{
    public class EndTurnDecision : IDecision
    {
        public static IDecision Instance { get; } = new EndTurnDecision();
    }
}
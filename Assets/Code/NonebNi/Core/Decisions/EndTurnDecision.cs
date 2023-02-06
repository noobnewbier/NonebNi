namespace NonebNi.Core.Decision
{
    public class EndTurnDecision : IDecision
    {
        public static IDecision Instance { get; } = new EndTurnDecision();
    }
}
namespace NonebNi.Core.Decisions
{
    public class ComboDecision : IDecision
    {
        public ComboDecision(params IDecision[] decisions)
        {
            Decisions = decisions;
        }

        public IDecision[] Decisions { get; }
    }
}
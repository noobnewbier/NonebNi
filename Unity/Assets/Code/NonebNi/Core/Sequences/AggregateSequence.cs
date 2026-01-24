namespace NonebNi.Core.Sequences
{
    public class AggregateSequence : ISequence
    {
        public readonly ISequence[] Sequences;

        public AggregateSequence(params ISequence[] sequences)
        {
            Sequences = sequences;
        }
    }
}
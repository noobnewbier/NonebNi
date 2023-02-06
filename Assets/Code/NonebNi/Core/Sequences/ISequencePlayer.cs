using System.Collections;
using System.Collections.Generic;

namespace NonebNi.Core.Sequences
{
    public interface ISequencePlayer
    {
        IEnumerator Play(IEnumerable<ISequence> sequences);
    }
}
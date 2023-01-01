using System.Collections.Generic;
using UnityEngine;

namespace NonebNi.Core.Sequences
{
    public interface ISequencePlayer
    {
        Coroutine Play(IEnumerable<ISequence> sequences);
    }
}
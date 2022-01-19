using UnityEngine;

namespace NonebNi.Core.Sequences
{
    public interface ISequencePlayer
    {
        Coroutine? Play(ISequence sequence);
    }
}
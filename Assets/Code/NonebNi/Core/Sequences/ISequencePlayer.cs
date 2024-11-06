using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace NonebNi.Core.Sequences
{
    public interface ISequencePlayer
    {
        UniTask Play(IEnumerable<ISequence> sequences);
    }
}
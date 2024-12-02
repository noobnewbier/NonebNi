using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Ui.Animation.Sequence;

namespace NonebNi.Ui.Animation
{
    public interface IPlayAnimation { }

    public interface IPlayAnimation<in T> : IPlayAnimation where T : IAnimSequence
    {
        UniTask Play(T sequence, CancellationToken ct = default);
    }
}
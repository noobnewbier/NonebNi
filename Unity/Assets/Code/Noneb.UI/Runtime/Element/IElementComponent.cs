using System.Threading;
using Cysharp.Threading.Tasks;

namespace Noneb.UI.Element
{
    public interface IElementComponent
    {
        public UniTask OnInit(CancellationToken ct) => UniTask.CompletedTask;

        public UniTask OnActivate() => UniTask.CompletedTask;

        public UniTask OnDeactivate() => UniTask.CompletedTask;
    }
}
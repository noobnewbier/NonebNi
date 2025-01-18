using Cysharp.Threading.Tasks;

namespace Noneb.UI.Element
{
    public interface IElementComponent
    {
        public UniTask OnInit() => UniTask.CompletedTask;

        public UniTask OnActivate() => UniTask.CompletedTask;

        public UniTask OnDeactivate() => UniTask.CompletedTask;
    }
}
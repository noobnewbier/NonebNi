using Cysharp.Threading.Tasks;

namespace Noneb.UI.View
{
    public interface IViewComponent
    {
        /// <summary>
        /// This is where you gather dependencies, load permanent(for the duration of the entire game) resources etc
        /// </summary>
        public UniTask OnViewInit() => UniTask.CompletedTask; //TODO: maybe I should get rid of this and let main-ish/entry point handle it.

        /// <summary>
        /// You are about to die now -> say your last word
        /// </summary>
        public UniTask OnViewTearDown() => UniTask.CompletedTask;

        /// <summary>
        /// Animation/transition, preferrably nothing logic related
        /// </summary>
        public UniTask OnViewEnter(INonebView? previousView, INonebView currentView) => UniTask.CompletedTask;

        /// <summary>
        /// Animation/transition, preferably nothing logic related
        /// </summary>
        public UniTask OnViewLeave(INonebView currentView, INonebView? nextView) => UniTask.CompletedTask;

        /// <summary>
        /// Tearing down input handler and event hook.
        /// Releasing temporary resources.
        /// </summary>
        public UniTask OnViewDeactivate() => UniTask.CompletedTask;

        /// <summary>
        /// Generic shenanigans to let implementer be type safe
        /// </summary>
        internal UniTask OnViewActivate(object? viewData) => UniTask.CompletedTask;
    }

    //TODO: work out editor/inspector -> you will need it for debugging.
    //TODO: next question -> handle GO instantiation/loadin and management
    public interface IViewComponent<in TViewData> : IViewComponent where TViewData : class
    {
        async UniTask IViewComponent.OnViewActivate(object? viewData)
        {
            var typedData = viewData as TViewData;
            await OnViewActivate(typedData);
        }

        /// <summary>
        /// Logic for initializing, so setting up input handler and hooking up events
        /// Temporary resources might also be loaded here.
        /// </summary>
        public UniTask OnViewActivate(TViewData? viewData) => UniTask.CompletedTask;
    }
}
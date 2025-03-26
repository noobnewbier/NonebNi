using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Noneb.UI.Element;

namespace Noneb.UI.View
{
    //TODO: feels horrible wtf?
    public interface INonebView
    {
        public enum InitializationState
        {
            PreInitialize,
            Initializing,
            Initialized
        }

        public string Name { get; }

        internal InitializationState InitState { get; set; }
        bool IsViewActive { get; }

        internal async UniTask Init()
        {
            //TODO: there has to be a better way than this crap. the proper way is to use lock and return the cached task but that seems like overkill...
            if (InitState != InitializationState.PreInitialize) return;

            //TODO: at some point more complicated logic for "activeness" and relevant state tracking?
            var handlers = FindViewComponents();
            var initTasks = handlers.Select(h => h.OnViewInit());
            InitState = InitializationState.Initializing;
            await UniTask.WhenAll(initTasks);

            InitState = InitializationState.Initialized;
        }

        internal async UniTask TearDown()
        {
            SetActive(false);

            //TODO: at some point more complicated logic for "activeness" and relevant state tracking?
            var handlers = FindViewComponents();
            var initTasks = handlers.Select(h => h.OnViewTearDown());
            await UniTask.WhenAll(initTasks);
        }

        //TODO: handle overlay
        //TODO: cancellation token? does it makes sense?
        internal async UniTask Enter(INonebView? previousView)
        {
            //TODO: what's the relationship with substacks here...
            //TODO: dk what i need more but feels like i missed sth

            var handlers = FindViewComponents();
            var enterTasks = handlers.Select(h => h.OnViewEnter(previousView));
            await UniTask.WhenAll(enterTasks);
        }

        internal async UniTask Leave(INonebView? nextView)
        {
            var handlers = FindViewComponents();
            var exitTasks = handlers.Select(h => h.OnViewLeave(nextView));
            await UniTask.WhenAll(exitTasks);
        }

        internal async UniTask Activate()
        {
            SetActive(true);

            var handlers = FindViewComponents();
            var activateTasks = handlers.Select(h => h.OnViewActivate());
            await UniTask.WhenAll(activateTasks);

            var childElements = FindChildElements();
            var elementActivateTasks = childElements.Select(c => c.Activate());
            await UniTask.WhenAll(elementActivateTasks);
        }

        internal async UniTask Deactivate()
        {
            SetActive(false);

            var handlers = FindViewComponents();
            var deactivateTasks = handlers.Select(h => h.OnViewDeactivate());
            await UniTask.WhenAll(deactivateTasks);

            var childElements = FindChildElements();
            var elementActivateTasks = childElements.Select(c => c.Deactivate());
            await UniTask.WhenAll(elementActivateTasks);
        }

        internal void SetActive(bool isActive);

        internal IEnumerable<IViewComponent> FindViewComponents();
        internal IEnumerable<NonebElement> FindChildElements();
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.Logging;
using UnityEngine;

namespace Noneb.UI.View
{
    /// <summary>
    /// This should be how we are interacting with the push/pop interaction.
    /// TODO: atm this only deal with the most basic of basics, we will "need" to expand on this.
    /// look at your notebook it has more details.
    /// </summary>
    public class UIStack : IAsyncDisposable //TODO: let stack handles game object as well
    {
        private readonly CancellationTokenSource _cts = new();

        /// <summary>
        /// Push/pop/replace's animation transition works asynchronously using channel, this happens they always work in sequence,
        /// no matter how rapid the request is coming in the transition is always in tact.
        /// Remember only the transition is asynchronous, the logic is synchronous -> _stack is always logically correct,
        /// and by extension there can't be any timing issue, at least logically, regarding values of CurrentView
        /// </summary>
        private readonly Channel<TransitionRequest> _requestChannel = Channel.CreateSingleConsumerUnbounded<TransitionRequest>();

        /// <summary>
        /// This field exists purely for debugging purposes, maybe we should put it in a good old ifdef but for now I can live with
        /// it.
        /// </summary>
        private readonly Queue<TransitionRequestInfo> _requestInfos = new();

        private readonly GameObject _root;

        //TODO: ct support...? I think I might be crying internally

        //TODO: we probs want to log everything that happens in a UI stack
        //TODO: could be useful for ui stack to have a name for debug purposes.

        private readonly Stack<ViewStack> _stack = new();

        //TODO: is a collection?
        private readonly Dictionary<string, UIStack> _subStacks = new(); //TODO: init/maintain/refresh this, seems like we can hook into unity's event life cycle somewhat? probs better to make this process explicit?

        public UIStack(GameObject root)
        {
            _root = root;
            RunRequests().Forget();
        }

        public INonebView? CurrentView
        {
            get
            {
                if (!_stack.TryPeek(out var current)) return null;

                return current.View;
            }
        }

        public IEnumerable<TransitionRequestInfo> RequestsInfo => _requestInfos.AsEnumerable();

        public async ValueTask DisposeAsync()
        {
            //TODO: these might need to be cancellable.

            // await ProcessRequest();
            _requestChannel.Writer.Complete();
            await _requestChannel.Reader.Completion;
            _cts.Cancel();
            _cts.Dispose();

            while (_stack.Any())
            {
                var (view, _) = _stack.Pop();
                await view.TearDown();
            }

            foreach (var stack in _subStacks.Values) await stack.DisposeAsync();
        }

        private async UniTask RunRequests()
        {
            await foreach (var request in _requestChannel.Reader.ReadAllAsync())
            {
                //todo: not sure why but this is inconsistent.
                //my guess is that the previous request is still running before the next one hit... we might need to split transition and the logic
                await request.Task.Invoke();
                request.IsDone = true;

                var info = _requestInfos.Dequeue();
                if (request.Info != info) Log.Error("Something went wrong, logically you should not get here as the queue should be parallel with the channel");
            }
        }

        public UIStack GetSubStack(string name)
        {
            if (!_subStacks.TryGetValue(name, out var subStack))
            {
                var subStackRoot = new GameObject(name);
                subStackRoot.transform.SetParent(_root.transform);

                _subStacks[name] = subStack = new UIStack(subStackRoot);
            }

            return subStack;
        }

        public IEnumerable<(string name, UIStack subStack)> GetSubStacks()
        {
            foreach (var (name, subStack) in _subStacks) yield return (name, subStack);
        }

        public IEnumerable<INonebView> GetViews() => _stack.Select(t => t.View);

        public async UniTask Push(IViewComponent component, object? viewData = null)
        {
            var view = FindViewFromComponent(component);
            if (view == null) return;

            await Push(view, viewData);
        }

        public async UniTask Push(INonebView nextView, object? viewData = null)
        {
            //TODO: which makes more sense/separate method or parameter overlay
            //Handles everything
            _ = _stack.TryPeek(out var currentViewStack);
            var nextViewStack = new ViewStack(nextView, viewData);
            _stack.Push(nextViewStack);

            var request = new TransitionRequest(() => Transition(currentViewStack.View, nextViewStack), new TransitionRequestInfo("Push", nextView.Name));
            await RequestTransition(request);
        }

        public async UniTask ReplaceCurrent(IViewComponent component, object? viewData = null)
        {
            var view = FindViewFromComponent(component);
            if (view == null) return;

            await ReplaceCurrent(view, viewData);
        }

        public async UniTask ReplaceCurrent(INonebView nextView, object? viewData = null)
        {
            //todo: how do I handle cases like this where I only need minimal change on the UI?
            if (CurrentView == nextView)
                // Nothing to replace - we are already the same chap
                return;

            _ = _stack.TryPop(out var currentViewStack);
            var nextViewStack = new ViewStack(nextView, viewData);
            _stack.Push(nextViewStack);

            var request = new TransitionRequest(() => Transition(currentViewStack?.View, nextViewStack), new TransitionRequestInfo("ReplaceCurrent", nextView.Name));
            await RequestTransition(request);
        }

        public async UniTask Pop()
        {
            if (!_stack.Any())
            {
                Log.Warning($"You are popping from {_root.name} when there's nothing around for you to pop");
                return;
            }


            var currentViewStack = _stack.Pop();
            _ = _stack.TryPeek(out var nextViewStack);

            var request = new TransitionRequest(() => Transition(currentViewStack.View, nextViewStack), new TransitionRequestInfo("Pop", null));
            await RequestTransition(request);
        }

        private async UniTask Transition(INonebView? currentView, ViewStack? nextViewStack)
        {
            if (currentView == null && nextViewStack == null) Log.Warning("This is an noop and likely not what you wanted");

            if (currentView != null) await LeaveCurrentView(currentView, nextViewStack?.View);

            if (nextViewStack != null)
            {
                await EnterNextView(currentView, nextViewStack.View, nextViewStack.ViewData);
            }
        }

        private INonebView? FindViewFromComponent(IViewComponent component)
        {
            if (component is not MonoBehaviour behaviour)
            {
                Log.Error("Unexpected typed - I can't work with non-concept-view component!");
                return null;
            }

            if (!behaviour.TryGetComponent<INonebView>(out var view))
            {
                Log.Error("Behaviour View can't work without a INonebView in the sibling components!");
                return null;
            }

            return view;
        }

        public bool IsCurrentComponent(IViewComponent component)
        {
            if (CurrentView == null) return false;

            var components = CurrentView.FindViewComponents();
            return components.Contains(component);
        }

        private async UniTask RequestTransition(TransitionRequest transitionRequest)
        {
            _requestInfos.Enqueue(transitionRequest.Info);
            if (!_requestChannel.Writer.TryWrite(transitionRequest)) Log.Error("Why is this happening?");

            await UniTask.WaitUntil(() => transitionRequest.IsDone, cancellationToken: _cts.Token);
        }

        private async UniTask LeaveCurrentView(INonebView currentView, INonebView? nextView)
        {
            await currentView.Leave(currentView, nextView);
            await currentView.Deactivate();
        }

        private async UniTask EnterNextView(INonebView? previousView, INonebView nextView, object? viewData)
        {
            if (nextView is NonebViewBehaviour component)
                component.transform.SetParent(_root.transform);

            await nextView.Init();
            await nextView.Activate(viewData);
            await nextView.Enter(previousView, nextView);
        }

        private record TransitionRequest(Func<UniTask> Task, TransitionRequestInfo Info)
        {
            public bool IsDone { get; set; }
        }

        /// <summary>
        /// This only exists for debug purposes, anything to do with the actual functionality goes to Request instead
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public record TransitionRequestInfo(string OperationName, string? ParameterViewName);

        private record ViewStack(INonebView View, object? ViewData);
    }
}
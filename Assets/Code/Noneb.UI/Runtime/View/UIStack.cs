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
    /// Note:
    /// - omg I really need a good code clean up, it's getting confusing
    /// - atm this only deal with the most basic of basics, we will "need" to expand on this.
    /// - look at your notebook it has more details.
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

        public UIStack(MonoBehaviour monoBehaviour) : this(monoBehaviour.gameObject) { }

        public UIStack(GameObject root)
        {
            _root = root;
            RunRequests().Forget();
        }

        public INonebView? CurrentView => CurrentViewStack?.View;

        private ViewStack? CurrentViewStack
        {
            get
            {
                if (!_stack.TryPeek(out var current)) return null;

                return current;
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
                var (view, _, _) = _stack.Pop();
                await view.TearDown();
            }

            foreach (var (_, __, subStacks) in _stack)
            foreach (var stack in subStacks.Values)
                await stack.DisposeAsync();
        }

        private async UniTask RunRequests()
        {
            await foreach (var request in _requestChannel.Reader.ReadAllAsync())
            {
                //todo: not sure why but this is inconsistent.
                //my guess is that the previous request is still running before the next one hit... we might need to split transition and the logic
                await request.Task.Invoke();
                if (!request.DoneTrigger.TrySetResult() && request.DoneTrigger.UnsafeGetStatus() != UniTaskStatus.Canceled) Log.Error("How did you get here, ever");

                var info = _requestInfos.Dequeue();
                if (request.Info != info) Log.Error("Something went wrong, logically you should not get here as the queue should be parallel with the channel");
            }
        }

        public UIStack GetSubStack(string name, IViewComponent component)
        {
            if (component is not MonoBehaviour behaviour)
            {
                Log.Error("Unexpected typed - I can't work with non-concept-view component!");
                return GetSubStack(name);
            }

            return GetSubStack(name, behaviour);
        }


        public UIStack GetSubStack(string name, MonoBehaviour monoBehaviour) => GetSubStack(name, monoBehaviour.gameObject);

        public UIStack GetSubStack(string name, GameObject? subStackRoot = null)
        {
            if (CurrentViewStack == null)
            {
                Push(new ConceptView("ConceptView")).Forget();
            }

            var subStacks = CurrentViewStack!.SubStacks;
            if (!subStacks.TryGetValue(name, out var subStack))
            {
                subStackRoot ??= new GameObject(name);
                subStackRoot.transform.SetParent(_root.transform);

                subStacks[name] = subStack = new UIStack(subStackRoot);
            }

            return subStack;
        }

        public IEnumerable<(string name, UIStack subStack)> GetSubStacks()
        {
            if (CurrentViewStack?.SubStacks == null) yield break;

            foreach (var (name, subStack) in CurrentViewStack.SubStacks)
                yield return (name, subStack);
        }

        public IEnumerable<INonebView> GetViews() => _stack.Select(t => t.View);

        //todo: this is not type safe, makes me sadge, any way to fix it? any more to the generic fuckery seems bad though  
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
            var nextViewStack = new ViewStack(nextView, viewData, new Dictionary<string, UIStack>());
            _stack.Push(nextViewStack);

            var request = new TransitionRequest(() => Transition(currentViewStack, nextViewStack, false), new TransitionRequestInfo("Push", nextView.Name));
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
            if (nextView == CurrentViewStack?.View && viewData == CurrentViewStack?.ViewData)
                // Nothing to replace - we are already the same chap
                return;

            _ = _stack.TryPop(out var currentViewStack);
            var nextViewStack = new ViewStack(nextView, viewData, new Dictionary<string, UIStack>());
            _stack.Push(nextViewStack);

            var request = new TransitionRequest(() => Transition(currentViewStack, nextViewStack, true), new TransitionRequestInfo("ReplaceCurrent", nextView.Name));
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

            var request = new TransitionRequest(() => Transition(currentViewStack, nextViewStack, true), new TransitionRequestInfo("Pop", null));
            await RequestTransition(request);
        }

        private async UniTask Transition(ViewStack? currentViewStack, ViewStack? nextViewStack, bool isCurrentStackRemoved)
        {
            if (currentViewStack == null && nextViewStack == null) Log.Warning("This is an noop and likely not what you wanted");

            if (currentViewStack != null) await LeaveCurrentView(currentViewStack, nextViewStack, isCurrentStackRemoved);

            if (nextViewStack != null)
            {
                await EnterNextView(currentViewStack, nextViewStack, nextViewStack.ViewData);
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

        private UniTask RequestTransition(TransitionRequest transitionRequest)
        {
            _requestInfos.Enqueue(transitionRequest.Info);
            if (!_requestChannel.Writer.TryWrite(transitionRequest)) Log.Error("Why is this happening?");

            _cts.Token.Register(() => transitionRequest.DoneTrigger.TrySetCanceled());
            return transitionRequest.DoneTrigger.Task;
        }

        private async UniTask LeaveCurrentView(ViewStack currentStack, ViewStack? nextStack, bool isCurrentStackRemoved)
        {
            // substacks get deactivated
            await UniTask.WhenAll(
                    currentStack.SubStacks.Values
                        .Select(s => s.CurrentView).Where(e => e != null).Select(e => e!)
                        .Select(v => v.Deactivate())
                )
                .SuppressCancellationThrow();

            // deactivate the root as well
            await currentStack.View.Deactivate().SuppressCancellationThrow();

            // leave in animation
            await currentStack.View.Leave(currentStack.View, nextStack?.View).SuppressCancellationThrow();

            if (isCurrentStackRemoved) await currentStack.View.TearDown().SuppressCancellationThrow();
        }

        private async UniTask EnterNextView(ViewStack? previousStack, ViewStack nextStack, object? viewData)
        {
            if (nextStack.View is NonebViewBehaviour component)
                component.transform.SetParent(_root.transform);

            // init when necessary
            await nextStack.View.Init();

            // activate the root
            await nextStack.View.Activate(viewData);

            // substacks get activated after root is online
            await UniTask.WhenAll(
                nextStack.SubStacks.Values
                    .Select(
                        s =>
                        {
                            if (s.CurrentViewStack?.View == null) return UniTask.CompletedTask;

                            return s.CurrentViewStack.View.Activate(s.CurrentViewStack.ViewData);
                        }
                    )
            );

            await nextStack.View.Enter(previousStack?.View, nextStack.View);
        }

        private record TransitionRequest(Func<UniTask> Task, TransitionRequestInfo Info)
        {
            public UniTaskCompletionSource DoneTrigger { get; } = new();
        }

        /// <summary>
        /// This only exists for debug purposes, anything to do with the actual functionality goes to Request instead
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public record TransitionRequestInfo(string OperationName, string? ParameterViewName);

        private record ViewStack(INonebView View, object? ViewData, Dictionary<string, UIStack> SubStacks);
    }
}
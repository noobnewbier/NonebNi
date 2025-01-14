using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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
        private readonly GameObject _root;

        //TODO: ct support...? I think I might be crying internally

        //TODO: we probs want to log everything that happens in a UI stack
        //TODO: could be useful for ui stack to have a name for debug purposes.

        private readonly Stack<INonebView> _stack = new();

        //TODO: is a collection?
        private readonly Dictionary<string, UIStack> _subStacks = new(); //TODO: init/maintain/refresh this, seems like we can hook into unity's event life cycle somewhat? probs better to make this process explicit?


        public UIStack(GameObject root)
        {
            _root = root;
            //TODO:
        }

        public INonebView? CurrentView
        {
            get
            {
                if (!_stack.TryPeek(out var current)) return null;

                return current;
            }
        }

        public async ValueTask DisposeAsync()
        {
            while (_stack.Any())
            {
                var view = _stack.Pop();
                await view.TearDown();
            }

            foreach (var stack in _subStacks.Values) await stack.DisposeAsync();
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

        public IEnumerable<INonebView> GetViews() => _stack;

        public async UniTask Push(INonebView nextView)
        {
            //TODO: which makes more sense/separate method or parameter overlay
            //Handles everything
            _ = _stack.TryPeek(out var currentView);
            _stack.Push(nextView);

            if (currentView != null)
            {
                await currentView.Leave(nextView);
                await currentView.Deactivate();
            }

            if (nextView is NonebUIComponent component)
                //TODO: code smell
                component.transform.SetParent(_root.transform);

            await nextView.Init();
            await nextView.Activate();
            await nextView.Enter(currentView);
        }

        public async UniTask ReplaceCurrent(INonebView nextView)
        {
            _ = _stack.TryPop(out var currentView);
            _stack.Push(nextView);

            if (currentView != null)
            {
                await currentView.Leave(nextView);
                await currentView.Deactivate();
            }

            await nextView.Activate();
            await nextView.Enter(currentView);
        }

        public async UniTask Pop()
        {
            if (!_stack.Any())
                //TODO: log?
                return;

            var currentView = _stack.Pop();
            _ = _stack.TryPeek(out var nextView);

            await currentView.Leave(nextView);
            await currentView.Deactivate();

            if (nextView != null)
            {
                // No need to call init here -> you must have called init when pushing.
                await nextView.Activate();
                await nextView.Enter(currentView);
            }
        }
    }
}
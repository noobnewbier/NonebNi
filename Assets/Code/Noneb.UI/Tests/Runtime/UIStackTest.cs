using System.Collections;
using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Noneb.UI.Tests
{
    [TestFixture, TestOf(typeof(UIStack))]
    public class UIStackTest
    {
        [UnityTest]
        public IEnumerator Push_AnyView_ChildAlso() => UniTask.ToCoroutine(
            async () =>
            {
                var stackRoot = new GameObject();
                var stack = new UIStack(stackRoot);

                var dummyViewObject = new GameObject();
                var view = dummyViewObject.AddComponent<NonebViewBehaviour>();

                var pushTask = stack.Push(view);
                Assert.That(stack.CurrentView, Is.SameAs(view));

                await pushTask;
            }
        );

        [UnityTest]
        public IEnumerator Push_AnyView_StackIsUpdatedImmediately() => UniTask.ToCoroutine(
            async () =>
            {
                var stackRoot = new GameObject();
                var stack = new UIStack(stackRoot);

                var dummyViewObject = new GameObject();
                var view = dummyViewObject.AddComponent<NonebViewBehaviour>();

                var pushTask = stack.Push(view);
                Assert.That(stack.CurrentView, Is.SameAs(view));

                await pushTask;
            }
        );

        [UnityTest]
        public IEnumerator Pop_AnyView_StackIsUpdatedImmediately() => UniTask.ToCoroutine(
            async () =>
            {
                var stackRoot = new GameObject();
                var stack = new UIStack(stackRoot);

                var dummyViewObject = new GameObject();
                var view = dummyViewObject.AddComponent<NonebViewBehaviour>();
                await stack.Push(view);

                var popTask = stack.Pop();
                Assert.That(stack.CurrentView, Is.Null);

                await popTask;
            }
        );

        [UnityTest]
        public IEnumerator Pop_StackIsEmpty_NothingHappens() => UniTask.ToCoroutine(
            async () =>
            {
                var stackRoot = new GameObject();
                var stack = new UIStack(stackRoot);

                await stack.Pop();
                Assert.DoesNotThrowAsync(async () => await stack.Pop());
            }
        );
    }
}
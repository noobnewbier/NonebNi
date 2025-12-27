using System.Collections;
using Cysharp.Threading.Tasks;
using Moq;
using Noneb.UI.View;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityUtils.TestUtils;

namespace Noneb.UI.Tests
{
    [TestFixture, TestOf(typeof(UIStack))]
    public class UIStackTest
    {
        [UnityTest]
        public IEnumerator Push_AnyView_StackIsUpdatedImmediately() => UniTask.ToCoroutine(
            async () =>
            {
                var stackRoot = new GameObject();
                var stack = new UIStack(stackRoot);

                var dummyViewObject = new GameObject();
                var view = dummyViewObject.AddComponent<NonebViewBehaviour>();

                // don't wait here -> it should be updated, *immediately*, before task is finished.
                var pushTask = stack.Push(view);
                Assert.That(stack.CurrentView, Is.SameAs(view));

                await pushTask;
            }
        );

        [UnityTest]
        public IEnumerator Leave_AnyView_SubStacksAreDeactivated() => UniTask.ToCoroutine(
            async () =>
            {
                var stackRoot = new GameObject();
                var stack = new UIStack(stackRoot);
                await stack.Push(Mock.Of<INonebView>());

                var mock = new Mock<INonebView>();
                await stack.GetSubStack("any").Push(mock.Object);

                await stack.Pop();
                mock.Verify(v => v.Deactivate());
            }
        );

        [UnityTest]
        public IEnumerator GetSubStack_WhenNoView_NewViewIsCreated() => UniTask.ToCoroutine(
            () =>
            {
                var stackRoot = new GameObject();
                var stack = new UIStack(stackRoot);

                _ = stack.GetSubStack("any");

                Assert.That(stack.CurrentView, Is.Not.Null);
                return UniTask.CompletedTask;
            }
        );

        [UnityTest]
        public IEnumerator Push_AnyView_SubStacksAreActivated() => UniTask.ToCoroutine(
            async () =>
            {
                var stackRoot = new GameObject();
                var stack = new UIStack(stackRoot);

                var mock = new Mock<INonebView>();
                var dummyData = new object();
                await stack.GetSubStack("any").Push(mock.Object, dummyData);

                await stack.Push(Mock.Of<INonebView>());
                await stack.Pop();

                mock.Verify(v => v.Activate(dummyData));
            }
        );

        [UnityTest]
        public IEnumerator Push_AnyView_CallOrderOnNewViewIsCorrect() => UniTask.ToCoroutine(
            async () =>
            {
                var stackRoot = new GameObject();
                var stack = new UIStack(stackRoot);

                var dummyViewData = new object();
                var mock = new Mock<INonebView>();

#pragma warning disable CS4014
                mock.SetUpCallInOrder(
                    v => v.Init(),
                    v => v.Activate(It.IsAny<object?>()),
                    v => v.Enter(null, mock.Object)
                );
#pragma warning restore CS4014

                await stack.Push(mock.Object, dummyViewData);

                mock.Verify();
            }
        );

        [UnityTest]
        public IEnumerator Replace_WhenNoViewExistsCurrently_CallOrderOnNewViewIsCorrect() => UniTask.ToCoroutine(
            async () =>
            {
                var stackRoot = new GameObject();
                var stack = new UIStack(stackRoot);

                var dummyViewData = new object();
                var mock = new Mock<INonebView>();

#pragma warning disable CS4014
                mock.SetUpCallInOrder(
                    v => v.Init(),
                    v => v.Activate(It.IsAny<object?>()),
                    v => v.Enter(null, mock.Object)
                );
#pragma warning restore CS4014

                await stack.ReplaceCurrent(mock.Object, dummyViewData);

                mock.Verify();
            }
        );


        [UnityTest]
        public IEnumerator Replace_AnyView_CallOrderOnNewViewIsCorrect() => UniTask.ToCoroutine(
            async () =>
            {
                var stackRoot = new GameObject();
                var stack = new UIStack(stackRoot);

                var existingView = Mock.Of<INonebView>();
                await stack.Push(existingView);

                var nextViewMock = new Mock<INonebView>();

#pragma warning disable CS4014
                nextViewMock.SetUpCallInOrder(
                    v => v.Init(),
                    v => v.Activate(It.IsAny<object?>()),
                    v => v.Enter(existingView, nextViewMock.Object)
                );

#pragma warning restore CS4014

                await stack.ReplaceCurrent(nextViewMock.Object);
                nextViewMock.Verify();
            }
        );

        [UnityTest]
        public IEnumerator Replace_AnyView_CallOrderOnExistingViewIsCorrect() => UniTask.ToCoroutine(
            async () =>
            {
                var stackRoot = new GameObject();
                var stack = new UIStack(stackRoot);

                var mock = new Mock<INonebView>();
                await stack.Push(mock.Object);

                var nextView = Mock.Of<INonebView>();

#pragma warning disable CS4014
                mock.SetUpCallInOrder(
                    v => v.Deactivate(),
                    v => v.Leave(mock.Object, nextView)
                );

#pragma warning restore CS4014

                await stack.ReplaceCurrent(nextView);
                mock.Verify();
            }
        );

        [UnityTest]
        public IEnumerator Push_AnyView_CallOrderOnExistingViewIsCorrect() => UniTask.ToCoroutine(
            async () =>
            {
                var stackRoot = new GameObject();
                var stack = new UIStack(stackRoot);

                var mock = new Mock<INonebView>();
                await stack.Push(mock.Object);

                var nextView = Mock.Of<INonebView>();

#pragma warning disable CS4014
                mock.SetUpCallInOrder(
                    v => v.Deactivate(),
                    v => v.Leave(mock.Object, nextView)
                );

#pragma warning restore CS4014

                await stack.Push(nextView);
                mock.Verify();
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
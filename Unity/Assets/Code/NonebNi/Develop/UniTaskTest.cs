using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NonebNi.Develop
{
    public class UniTaskTest : MonoBehaviour
    {
        private bool _isDone;

        [ContextMenu(nameof(TestTcs))]
        public void TestTcs()
        {
            var tcs = new UniTaskCompletionSource();

            async void Do()
            {
                var startFrame = Time.frameCount;

                await tcs.Task;

                var currentFrame = Time.frameCount;
                Debug.Log(startFrame == currentFrame);
            }


            Do();
            tcs.TrySetResult();
        }


        [ContextMenu(nameof(TestCt))]
        public void TestCt()
        {
            var tcs = new UniTaskCompletionSource();

            async UniTask RunningTask(CancellationToken ct)
            {
                var startFrame = Time.frameCount;
                try
                {
                    await UniTask.WaitForSeconds(199, cancellationToken: ct, cancelImmediately: true);
                }
                finally
                {
                    var currentFrame = Time.frameCount;
                    Debug.Log(startFrame == currentFrame);
                    Debug.Log(startFrame);
                    Debug.Log(currentFrame);
                }
            }

            async void Do()
            {
                var cts = new CancellationTokenSource();
                RunningTask(cts.Token).Forget();

                cts.Cancel();
            }

            Do();
            tcs.TrySetResult();
        }

        [ContextMenu(nameof(TestWaitUntil))]
        public void TestWaitUntil()
        {
            _isDone = false;

            async void Do()
            {
                var startFrame = Time.frameCount;

                await UniTask.WaitUntil(() => _isDone);

                var currentFrame = Time.frameCount;
                Debug.Log(startFrame == currentFrame);
            }


            Do();
            _isDone = true;
        }
    }
}
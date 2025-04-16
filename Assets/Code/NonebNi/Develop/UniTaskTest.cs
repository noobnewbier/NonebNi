using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NonebNi.Develop
{
    public class UniTaskTest : MonoBehaviour
    {
        [SerializeField] private int times;

        private readonly Channel<int> channel = Channel.CreateSingleConsumerUnbounded<int>();

        private void Awake()
        {
            async UniTaskVoid Do()
            {
                await foreach (var i in channel.Reader.ReadAllAsync()) Debug.Log($"{Time.frameCount}");
            }

            Do().Forget();
        }

        [ContextMenu(nameof(Publish))]
        public void Publish()
        {
            for (var i = 0; i < times; i++) channel.Writer.TryWrite(1);
        }
    }
}
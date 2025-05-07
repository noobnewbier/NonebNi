using Cysharp.Threading.Tasks;

namespace Noneb.UI.View
{
    /// <summary>
    /// Really just a wrapper of TCS, I can regret later though
    /// </summary>
    public sealed class UIInputReader<T>
    {
        private UniTaskCompletionSource<T>? _tcs;

        public void Write(T data)
        {
            _tcs ??= new UniTaskCompletionSource<T>();
            if (_tcs.TrySetResult(data)) return;

            // try again - this time with a new tcs
            _tcs = null;
            Write(data);
        }

        public async UniTask<T> Read()
        {
            // If tcs is not null -> we are writing before read is triggered -> fine if you are so eager we will just give you that.
            _tcs ??= new UniTaskCompletionSource<T>();
            var toReturn = await _tcs.Task;

            _tcs = null;
            return toReturn;
        }
    }
}
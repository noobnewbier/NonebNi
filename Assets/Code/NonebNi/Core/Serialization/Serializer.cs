using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

namespace NonebNi.Core.Serialization
{
    public interface ISerializer
    {
        UniTask<string> Serialize(object data);
        UniTask<T?> Deserialize<T>(string json);
    }

    public class Serializer : ISerializer
    {
        public UniTask<string> Serialize(object data)
        {
            var task = UniTask.RunOnThreadPool(
                () => JsonConvert.SerializeObject(data, Formatting.Indented)
            );
            return task;
        }

        public UniTask<T?> Deserialize<T>(string json)
        {
            var task = UniTask.RunOnThreadPool(
                () => JsonConvert.DeserializeObject<T>(json)
            );

            return task;
        }
    }
}
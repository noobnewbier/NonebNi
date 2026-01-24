using Cysharp.Threading.Tasks;
using NonebNi.Core.FileSystem;
using NonebNi.Core.Serialization;

namespace NonebNi.Core.Save
{
    public interface ISaveService
    {
        UniTask Save(object data, string filepath);
        UniTask<T?> Load<T>(string filepath);
    }

    public class SaveService : ISaveService
    {
        private readonly IFileSystem _fileSystem;
        /*
         * Note:
         *
         * Haven't implemented save slot yet. At the moment you have one save slot and everything is there.
         * We should split out global, and per save slot files.
         */

        private readonly ISerializer _serializer;


        public SaveService(ISerializer serializer, IFileSystem fileSystem)
        {
            _serializer = serializer;
            _fileSystem = fileSystem;
        }

        public async UniTask Save(object data, string filepath)
        {
            var json = await _serializer.Serialize(data);
            await _fileSystem.Write(filepath, json);
        }

        public async UniTask<T?> Load<T>(string filepath)
        {
            var json = await _fileSystem.Read(filepath);
            var data = await _serializer.Deserialize<T>(json);

            return data;
        }
    }
}
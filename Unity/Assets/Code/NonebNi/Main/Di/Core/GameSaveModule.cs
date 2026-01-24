using NonebNi.Core.FileSystem;
using NonebNi.Core.Save;
using NonebNi.Core.Serialization;
using StrongInject;

namespace NonebNi.Main.Di.Core
{
    [
        Register(typeof(FileSystem), typeof(IFileSystem)),
        Register(typeof(Serializer), typeof(ISerializer)),
        Register(typeof(SaveService), typeof(ISaveService))
    ]
    public class GameSaveModule { }
}
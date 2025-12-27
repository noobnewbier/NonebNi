using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NonebNi.Core.FileSystem
{
    public interface IFileSystem
    {
        UniTask Write(string data, string filepath, CancellationToken ct = default);
        UniTask<string> Read(string filepath, CancellationToken ct = default);
    }

    public class FileSystem : IFileSystem
    {
        /*
         * Note:
         * - Should probably do the "try-if-fail-delay-try-again" maniac.
         *
         * - I thought about making sure write/read can only happen when no other peeps are doing it, but it's too much of a hassle.
         * We can do that a bit later when it actually become a problem
         */

        public async UniTask Write(string data, string filepath, CancellationToken ct = default)
        {
            var fi = await GetFile(filepath);
            await using var writer = new StreamWriter(fi.Open(FileMode.Truncate));
            var mem = new ReadOnlyMemory<char>(data.ToCharArray());
            await writer.WriteAsync(mem, ct);
        }

        public async UniTask<string> Read(string filepath, CancellationToken ct = default)
        {
            var fi = await GetFile(filepath);
            using var streamReader = new StreamReader(fi.OpenRead());
            var content = await UniTask.RunOnThreadPool(async () => await streamReader.ReadToEndAsync(), cancellationToken: ct) ?? string.Empty;

            return content;
        }

        private static async UniTask<FileInfo> GetFile(string subPath)
        {
            var root = Application.persistentDataPath;
            var path = Path.Combine(root, subPath);

            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory)) Directory.CreateDirectory(directory);

            if (!File.Exists(path)) await File.Create(path).DisposeAsync();

            var fileInfo = new FileInfo(path);
            return fileInfo;
        }
    }
}
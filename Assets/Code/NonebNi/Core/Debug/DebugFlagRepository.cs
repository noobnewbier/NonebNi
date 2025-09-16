using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Save;
using StrongInject;

namespace NonebNi.Core.Debug
{
    public interface IDebugFlagRepository
    {
        bool Get(string flagName);
        void Set(string flagName, bool value);
        IEnumerable<(string flag, bool value)> GetAll();
    }

    public class DebugFlagRepository : IDebugFlagRepository, IRequiresAsyncInitialization
    {
        private const string SaveFilePath = "DebugFlags.json";
        private readonly ISaveService _saveService;

        private DebugFlags _debugFlags = new();

        public DebugFlagRepository(ISaveService saveService)
        {
            _saveService = saveService;
        }

        public bool Get(string flagName) => _debugFlags.Flags.GetValueOrDefault(flagName, false);

        public void Set(string flagName, bool value)
        {
            _debugFlags.Flags[flagName] = value;

            // Forget(), and regret later.
            _saveService.Save(_debugFlags, SaveFilePath).Forget();
        }

        public IEnumerable<(string flag, bool value)> GetAll()
        {
            foreach (var (key, value) in _debugFlags.Flags) yield return (key, value);
        }

        public async ValueTask InitializeAsync()
        {
            /*
             * Note:
             * At some point we might want a more well-defined post ctor initialization mechanism,
             * or a way that ctor create this and it will be in a "valid" state right off the bat
             * notably one that can handle dependency chain
             * But now this is good enough for now.
             */
            var loadData = await _saveService.Load<DebugFlags>(SaveFilePath);
            if (loadData == null) return;

            _debugFlags = loadData;
        }
    }
}
using NonebNi.Core.Level;

namespace NonebNi.LevelEditor.Level.Data
{
    public class EditorLevelData
    {
        public WorldConfigData WorldConfig { get; }
        public EditorMap Map { get; }
        public string LevelName { get; }


        public EditorLevelData(WorldConfigData worldConfig, EditorMap map, string levelName)
        {
            WorldConfig = worldConfig;
            Map = map;
            LevelName = levelName;
        }

        public LevelData ToLevelData() => new LevelData(WorldConfig, Map.ToMap(), LevelName);
    }
}
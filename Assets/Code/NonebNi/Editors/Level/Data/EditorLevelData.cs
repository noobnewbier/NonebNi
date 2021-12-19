using NonebNi.Core.Level;

namespace NonebNi.Editors.Level.Data
{
    public class EditorLevelData
    {
        public WorldConfigData WorldConfig { get; }
        public EditorMap Map { get; }

        public EditorLevelData(WorldConfigData worldConfig, EditorMap map)
        {
            WorldConfig = worldConfig;
            Map = map;
        }
    }
}
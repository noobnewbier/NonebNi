using NonebNi.Core.Entities;
using NonebNi.Core.Level;
using NonebNi.LevelEditor.Level.Maps;

namespace NonebNi.LevelEditor.Level
{
    public class EditorLevelData
    {
        public EditorLevelData(WorldConfigData worldConfig, EditorMap map, string levelName, Faction[] factions)
        {
            WorldConfig = worldConfig;
            Map = map;
            LevelName = levelName;
            Factions = factions;
        }

        public WorldConfigData WorldConfig { get; }
        public EditorMap Map { get; }
        public string LevelName { get; }
        public Faction[] Factions { get; }

        public LevelData ToLevelData() => new(WorldConfig, Map.ToMap(), LevelName, Factions);
    }
}
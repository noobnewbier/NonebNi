using NonebNi.Core.Factions;
using NonebNi.Core.Level;
using NonebNi.LevelEditor.Level.Maps;
using NonebNi.Terrain;

namespace NonebNi.LevelEditor.Level
{
    public class EditorLevelData
    {
        public EditorLevelData(TerrainConfigData terrainConfig, EditorMap map, string levelName, Faction[] factions)
        {
            TerrainConfig = terrainConfig;
            Map = map;
            LevelName = levelName;
            Factions = factions;
        }

        public TerrainConfigData TerrainConfig { get; }
        public EditorMap Map { get; }
        public string LevelName { get; }
        public Faction[] Factions { get; }

        public LevelData ToLevelData() => new(Map.ToMap(), LevelName, Factions);
    }
}
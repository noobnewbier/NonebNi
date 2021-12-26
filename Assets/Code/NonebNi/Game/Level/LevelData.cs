using System;
using Code.NonebNi.Game.Maps;
using UnityEngine;

namespace Code.NonebNi.Game.Level
{
    [Serializable]
    public class LevelData
    {
        [SerializeField] private string levelName;
        [SerializeField] private WorldConfigData worldConfig;
        [SerializeField] private Map map;

        public LevelData(WorldConfigData worldConfig, Map map, string levelName)
        {
            this.worldConfig = worldConfig;
            this.map = map;
            this.levelName = levelName;
        }

        public string LevelName => levelName;

        public WorldConfigData WorldConfig => worldConfig;

        public Map Map => map;
    }
}
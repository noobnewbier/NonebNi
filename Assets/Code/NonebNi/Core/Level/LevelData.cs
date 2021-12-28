using System;
using NonebNi.Core.Maps;
using UnityEngine;

namespace NonebNi.Core.Level
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
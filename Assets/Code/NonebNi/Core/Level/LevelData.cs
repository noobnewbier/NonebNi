using System;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using UnityEngine;

namespace NonebNi.Core.Level
{
    [Serializable]
    public class LevelData
    {
        [SerializeField] private string levelName;
        [SerializeField] private WorldConfigData worldConfig;
        [SerializeField] private Faction[] factions;

        [SerializeField] private Map map;

        public LevelData(WorldConfigData worldConfig, Map map, string levelName, Faction[] factions)
        {
            this.worldConfig = worldConfig;
            this.map = map;
            this.levelName = levelName;
            this.factions = factions;
        }

        public Faction[] Factions => factions;

        public string LevelName => levelName;

        public WorldConfigData WorldConfig => worldConfig;

        public Map Map => map;
    }
}
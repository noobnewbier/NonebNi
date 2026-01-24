using System;
using NonebNi.Core.Factions;
using NonebNi.Core.Maps;
using UnityEngine;

namespace NonebNi.Core.Level
{
    [Serializable]
    public class LevelData
    {
        [SerializeField] private string levelName;
        [SerializeField] private Faction[] factions;

        [SerializeField] private Map map;

        public LevelData(Map map, string levelName, Faction[] factions)
        {
            this.map = map;
            this.levelName = levelName;
            this.factions = factions;
        }

        public Faction[] Factions => factions;

        public string LevelName => levelName;


        public Map Map => map;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NonebNi.Core.Stats
{
    [Serializable]
    public class StatsCollection
    {
        [SerializeField] private List<Stat> stats;

        public StatsCollection(IEnumerable<Stat> stats)
        {
            this.stats = new List<Stat>(stats);
        }

        public StatsCollection(params Stat[] stats) : this((IEnumerable<Stat>)stats) { }

        public (bool success, Stat stat) FindStat(string id)
        {
            var targetStat = stats.FirstOrDefault(s => s.ID == id);
            if (targetStat == null) return (false, Stat.Invalid);

            return (true, targetStat);
        }

        public (bool success, int value) GetValue(string id)
        {
            var (success, stat) = FindStat(id);
            return (success, stat.CurrentValue);
        }

        public (bool success, int value) GetMaxValue(string id)
        {
            var (success, stat) = FindStat(id);
            return (success, stat.MaxValue);
        }

        public bool SetValue(string id, int value)
        {
            var (success, stat) = FindStat(id);
            if (!success) return false;

            stat.CurrentValue = value;
            return true;
        }

        public bool SetMaxValue(string id, int value)
        {
            var (success, stat) = FindStat(id);
            if (!success) return false;

            stat.MaxValue = value;
            return true;
        }

        public bool AddStat(Stat stat)
        {
            var (foundDuplicates, _) = FindStat(stat.ID);
            if (foundDuplicates) return false;

            stats.Add(stat);
            return true;
        }

        public bool CreateStat(string id, int currentValue, int minValue = -1, int maxValue = -1)
        {
            var newStat = new Stat(id, minValue, maxValue)
            {
                CurrentValue = currentValue
            };

            return AddStat(newStat);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NonebNi.Core.Stats
{
    [Serializable]
    public class StatsCollection : ISerializationCallbackReceiver
    {
        public enum PayCostError
        {
            StatNotFound,
            NotEnoughValue
        }

        [SerializeField] private List<Stat> stats;

        public StatsCollection(IEnumerable<Stat> stats)
        {
            this.stats = new List<Stat>();

            foreach (var stat in stats) AddStat(stat);
        }

        public StatsCollection(params Stat[] stats) : this((IEnumerable<Stat>)stats) { }
        public event Action<Stat>? StatUpdated;

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
            stat.ValueChanged += OnValueChanged;
            return true;
        }

        private void OnValueChanged(Stat updatedStat)
        {
            StatUpdated?.Invoke(updatedStat);
        }

        public bool CreateStat(string id, int currentValue, int minValue = -1, int maxValue = -1)
        {
            var newStat = new Stat(id, minValue, maxValue)
            {
                CurrentValue = currentValue
            };

            return AddStat(newStat);
        }

        public PayCostError? CheckCanPayCost(StatCost cost)
        {
            var (success, stat) = FindStat(cost.StatId);
            if (!success) return PayCostError.StatNotFound;

            //todo:
            //under the current model we can kill ourself by paying for cost,
            //we might need to introduce some way to say "this stat can go to zero but that stat cannot"

            if (stat.CurrentValue - stat.MinValue < cost.Cost) return PayCostError.NotEnoughValue;

            return null;
        }

        public PayCostError? PayCost(StatCost cost)
        {
            var error = CheckCanPayCost(cost);
            if (error != null) return error;

            var (success, stat) = FindStat(cost.StatId);
            if (!success) return PayCostError.StatNotFound;

            stat.CurrentValue -= cost.Cost;
            return null;
        }

        #region Save/Load

        /*
         * Wbn if we don't need this, mh2 model doesn't need it as they've got the whole init shenanigans in init process
         * maybe we can mimic it?
         */
        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            foreach (var stat in stats) stat.ValueChanged += OnValueChanged;
        }

        #endregion
    }
}
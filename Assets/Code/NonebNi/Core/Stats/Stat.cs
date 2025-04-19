using System;
using System.Diagnostics;
using UnityEngine;

namespace NonebNi.Core.Stats
{
    //TODO: stat modifier
    //TODO: could use an inspector?
    //TODO: at some point, stat would need to share sprites and loc sheet, and we will hate everything in the world, but that's okay, future me. (from past me with love)

    /// <summary>
    /// Does not support negative
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{ID}: {currentValue} [{MinValue} to {MaxValue}]")]
    public class Stat
    {
        [field: SerializeField] public string ID { get; private set; }
        [SerializeField] private int currentValue;
        [field: SerializeField] public int MinValue { get; set; } = -1;
        [field: SerializeField] public int MaxValue { get; set; } = -1;

        public Stat(string id)
        {
            ID = id;
        }

        public Stat(string id, int minValue, int maxValue)
        {
            ID = id;
            MinValue = minValue;
            MaxValue = maxValue;
            CurrentValue = maxValue;
        }

        public static Stat Invalid { get; } = new("invalid-stat", -1, -1);


        public int CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = value;
                ClampWithMinValue();
                ClampWithMaxValue();
            }
        }

        public bool HasMaxLimit => MaxValue != -1;

        private bool HasMinLimit => MinValue != -1;

        private void ClampWithMaxValue()
        {
            if (!HasMaxLimit) return;

            if (currentValue <= MaxValue) return;

            currentValue = MaxValue;
        }

        private void ClampWithMinValue()
        {
            if (!HasMinLimit) return;

            if (currentValue >= MinValue) return;

            currentValue = MinValue;
        }
    }
}
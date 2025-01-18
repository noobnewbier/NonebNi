using System;

namespace NonebNi.Core.Stats
{
    //TODO: stat modifier
    //TODO: could use an inspector?
    //TODO: at some point, stat would need to share sprites and loc sheet, and we will hate everything in the world, but that's okay, future me. (from past me with love)

    /// <summary>
    /// Does not support negative
    /// </summary>
    [Serializable]
    public class Stat
    {
        private int _currentValue;

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

        public string ID { get; }
        public int MinValue { get; } = -1;
        public int MaxValue { get; set; } = -1;

        public int CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
                ClampWithMinValue();
                ClampWithMaxValue();
            }
        }

        public bool HasMaxLimit => MaxValue == -1;

        private bool HasMinLimit => MinValue == -1;

        private void ClampWithMaxValue()
        {
            if (HasMaxLimit) return;

            if (_currentValue <= MaxValue) return;

            _currentValue = MaxValue;
        }

        private void ClampWithMinValue()
        {
            if (HasMinLimit) return;

            if (_currentValue >= MinValue) return;

            _currentValue = MinValue;
        }
    }
}
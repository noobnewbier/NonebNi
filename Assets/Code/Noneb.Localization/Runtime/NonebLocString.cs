using System;
using UnityEngine;

namespace Noneb.Localization.Runtime
{
    [Serializable]
    public record NonebLocString
    {
        [SerializeField] private string debugString = string.Empty;

        public NonebLocString(string value)
        {
            debugString = value;
        }

        public static NonebLocString Default { get; } = new("default");

        /*
         * TODO:
         * finish the implementation instead of having only a debug version.
         */

        public string GetLocalized() => debugString;

        public static implicit operator NonebLocString(string debugValue) => new(debugValue);
    }
}
using System;
using UnityEngine;

namespace NonebNi.Core.Factions
{
    [Serializable]
    public record Faction(string Id, bool IsPlayerControlled, params string[] Allies)
    {
        [field: SerializeField] public string Id { get; private set; } = string.Empty;
        [field: SerializeField] public bool IsPlayerControlled { get; private set; }
        [field: SerializeField] public string[] Allies { get; private set; } = Array.Empty<string>();
    }
}
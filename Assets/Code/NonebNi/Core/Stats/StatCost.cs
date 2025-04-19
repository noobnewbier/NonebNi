using System;
using UnityEngine;

namespace NonebNi.Core.Stats
{
    [Serializable]
    public record StatCost(string StatId, int Cost)
    {
        [field: SerializeField] public string StatId { get; private set; } = StatId;
        [field: SerializeField] public int Cost { get; private set; } = Cost;
    }
}
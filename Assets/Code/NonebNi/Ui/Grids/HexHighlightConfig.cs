using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils.Serialization;

namespace NonebNi.Ui.Grids
{
    [Serializable]
    public class HexHighlightConfig
    {
        [SerializeField] private SerializableDictionary<HighlightVariation, HexHighlight> idAndHighlights = new();

        public HexHighlight? FindHighlightPrefab(HighlightVariation id) =>
            idAndHighlights.TryGetValue(id, out var result) ?
                result :
                null;

        public IEnumerable<(HighlightVariation variation, HexHighlight highlight)> GetAll()
        {
            foreach (var (variation, value) in idAndHighlights) yield return (variation, value);
        }
    }
}
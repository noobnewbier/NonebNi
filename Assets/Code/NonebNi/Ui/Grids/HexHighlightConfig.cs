using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtils.Serialization;

namespace NonebNi.Ui.Grids
{
    [Serializable]
    public class HexHighlightConfig
    {
        [SerializeField] private SerializableDictionary<string, HexHighlight> idAndHighlights = new();

        public HexHighlight? FindHighlightPrefab(string id) =>
            idAndHighlights.TryGetValue(id, out var result) ?
                result :
                null;

        public IEnumerable<(string id, HexHighlight highlight)> GetAll()
        {
            foreach (var (key, value) in idAndHighlights) yield return (key, value);
        }
    }
}
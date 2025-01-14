using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Terrain;
using UnityUtils.Pooling;

namespace NonebNi.Ui.Grids
{
    public interface IHexHighlighter : IDisposable
    {
        /// <summary>
        /// If same id -> remove existing and recreate
        /// </summary>
        void AddHighlight(IEnumerable<Coordinate> coords, string id);

        /// <summary>
        /// if id == null -> nuke all highlight on specified coordinate
        /// </summary>
        void RemoveHighlight(IEnumerable<Coordinate> coords, string? id = null);

        void RemoveHighlight(string id);
        void ClearAll();
        void AddHighlight(Coordinate coord, string id);
        void RemoveHighlight(Coordinate coord, string? id = null);
    }

    //TODO: use some vfx so visually stuffs looks appealing
    public class HexHighlighter : IHexHighlighter
    {
        private readonly ICoordinateAndPositionService _coordinateAndPositionService;

        /// <summary>
        /// Not an efficient implementation but we can fix later once requirement is clear, current probs:
        /// 1. duplicated highlight on the same hex when there's multiple req with different id
        /// 2. inefficient iteration although it's probably not that bad
        /// </summary>
        private readonly Dictionary<Coordinate, Dictionary<string, HexHighlight>> _highlightMappings = new();

        private readonly BehaviourPool<HexHighlight> _pool;

        public HexHighlighter(HexHighlight highlightPrefab, ICoordinateAndPositionService coordinateAndPositionService)
        {
            _coordinateAndPositionService = coordinateAndPositionService;
            _pool = new BehaviourPool<HexHighlight>(highlightPrefab);
        }

        //TODO: highlight variation and id.
        public void AddHighlight(IEnumerable<Coordinate> coords, string id)
        {
            foreach (var coord in coords) AddHighlight(coord, id);
        }

        public void RemoveHighlight(IEnumerable<Coordinate> coords, string? id = null)
        {
            foreach (var coord in coords) RemoveHighlight(coord, id);
        }

        public void RemoveHighlight(string id)
        {
            var coordWithHighlights = _highlightMappings.Keys;
            RemoveHighlight(coordWithHighlights, id);
        }

        public void ClearAll()
        {
            var coordWithHighlights = _highlightMappings.Keys;
            RemoveHighlight(coordWithHighlights);
        }

        public void Dispose()
        {
            _pool.Dispose();
        }

        public void AddHighlight(Coordinate coord, string id)
        {
            if (!_highlightMappings.TryGetValue(coord, out var idToHighlight)) _highlightMappings[coord] = idToHighlight = new Dictionary<string, HexHighlight>();

            //Remove duplicates if any
            RemoveHighlight(coord, id);

            var newHighlight = _pool.Get();
            newHighlight.transform.position = _coordinateAndPositionService.FindPosition(coord);
            idToHighlight[id] = newHighlight;
        }

        public void RemoveHighlight(Coordinate coord, string? id = null)
        {
            if (!_highlightMappings.TryGetValue(coord, out var idToHighlight)) return;

            if (id == null)
            {
                var keys = idToHighlight.Keys.ToArray();
                foreach (var key in keys) DoRemove(key);

                return;
            }

            DoRemove(id);
            return;

            void DoRemove(string key)
            {
                if (!idToHighlight.Remove(key, out var highlight)) return;

                _pool.Release(highlight);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Terrain;
using Priority_Queue;
using Unity.Logging;
using UnityEngine;
using UnityUtils.Pooling;

namespace NonebNi.Ui.Grids
{
    /// <summary>
    /// All public API should be idempotent -> calling the same API twice won't have a different result
    /// </summary>
    public interface IHexHighlighter : IDisposable
    {
        /// <summary>
        /// If same id -> remove existing and recreate
        /// </summary>
        void RequestHighlight(IEnumerable<Coordinate> coords, string requestId, string highlightId);

        /// <summary>
        /// if id == null -> nuke all highlight on specified coordinate
        /// </summary>
        void RemoveRequest(IEnumerable<Coordinate> coords, string? requestId = null);

        void RemoveRequest(params string[] requestIds);
        void RemoveRequest(string requestId);
        void RequestHighlight(Coordinate coord, string requestId, string highlightId);
        void RemoveRequest(Coordinate coord, string? requestId = null);
        void ClearAll();
    }

    //TODO: use some vfx so visually stuffs looks appealing
    public class HexHighlighter : IHexHighlighter
    {
        private static readonly Dictionary<string, int> RequestPriority = new()
        {
            [HighlightRequestId.TargetSelection] = 0,
            [HighlightRequestId.TileInspection] = 1,
            [HighlightRequestId.AreaHint] = 2
        };

        private readonly ICoordinateAndPositionService _coordinateAndPositionService;

        private readonly HexHighlightConfig _highlightConfig;

        /// <summary>
        /// Not an efficient implementation but we can fix later once requirement is clear, current probs:
        /// 1. duplicated highlight on the same hex when there's multiple req with different id
        /// 2. inefficient iteration although it's probably not that bad
        /// </summary>
        private readonly Dictionary<Coordinate, ResponseRequest> _highlightMappings = new();

        private readonly Dictionary<string, BehaviourPool<HexHighlight>> _highlightPools = new();
        private readonly TerrainConfigData _terrainConfig;

        public HexHighlighter(ICoordinateAndPositionService coordinateAndPositionService, HexHighlightConfig highlightConfig, TerrainConfigData terrainConfig)
        {
            _coordinateAndPositionService = coordinateAndPositionService;
            _highlightConfig = highlightConfig;
            _terrainConfig = terrainConfig;
        }

        //TODO: highlight variation and id.
        public void RequestHighlight(IEnumerable<Coordinate> coords, string requestId, string highlightId)
        {
            foreach (var coord in coords) RequestHighlight(coord, requestId, highlightId);
        }

        public void RemoveRequest(IEnumerable<Coordinate> coords, string? requestId = null)
        {
            var coordinates = coords.ToArray();
            foreach (var coord in coordinates) RemoveRequest(coord, requestId);
        }

        public void RemoveRequest(params string[] requestIds)
        {
            foreach (var id in requestIds) RemoveRequest(id);
        }

        public void RemoveRequest(string requestId)
        {
            var coordWithHighlights = _highlightMappings.Keys;
            RemoveRequest(coordWithHighlights, requestId);
        }

        public void ClearAll()
        {
            var coordWithHighlights = _highlightMappings.Keys;
            RemoveRequest(coordWithHighlights);
        }

        public void Dispose()
        {
            foreach (var pool in _highlightPools.Values) pool.Dispose();
        }

        public void RequestHighlight(Coordinate coord, string requestId, string highlightId)
        {
            if (!_highlightMappings.TryGetValue(coord, out var responseRequest))
                _highlightMappings[coord] = responseRequest = new ResponseRequest();

            // Overriding existing request -> everything will be good after we refresh
            var request = responseRequest.Requests.FirstOrDefault(r => r.RequestId == requestId);
            if (request == null)
            {
                var priority = RequestPriority.GetValueOrDefault(highlightId, 0);
                request = new HighlightRequest(requestId);
                responseRequest.Requests.Enqueue(request, priority);
            }

            request.HighlightId = highlightId;

            ServeRequest(coord);
        }

        public void RemoveRequest(Coordinate coord, string? requestId = null)
        {
            if (!_highlightMappings.TryGetValue(coord, out var responseRequest)) return;

            if (requestId == null)
            {
                // Null -> user want to remove nuclear everything
                responseRequest.Requests.Clear();
            }
            else
            {
                // If the user do want to remove something -> find a request with matching id and remove it
                var matchingRequest = responseRequest.Requests.FirstOrDefault(r => r.RequestId == requestId);
                if (matchingRequest != null)
                    // No don't even look the library code throws exception if you haven't enqueued it
                    responseRequest.Requests.Remove(matchingRequest);
            }

            ServeRequest(coord);
        }

        private void ServeRequest(Coordinate coordinate)
        {
            if (!_highlightMappings.TryGetValue(coordinate, out var responseRequests)) return;

            var currentReq = responseRequests.Requests.FirstOrDefault();
            if (currentReq == null)
            {
                Cleanup(coordinate);
                return;
            }

            var currentResponse = responseRequests.Response;
            if (currentResponse?.CurrentHighlight != null)
            {
                var prevPrefabPool = GetOrCreatePool(currentResponse.HighlightId);
                prevPrefabPool.Release(currentResponse.CurrentHighlight);
            }

            var pool = GetOrCreatePool(currentReq.HighlightId);
            var newHighlight = pool.Get();
            newHighlight.transform.position = _coordinateAndPositionService.FindPosition(coordinate);
            newHighlight.Draw(_terrainConfig.InnerRadius);

            responseRequests.Response = new HighlightResponse(currentReq.HighlightId, newHighlight);
        }

        private void Cleanup(Coordinate coordinate)
        {
            if (!_highlightMappings.TryGetValue(coordinate, out var responseRequest)) return;

            if (responseRequest.Requests.Any()) return;

            var response = responseRequest.Response;
            if (response?.CurrentHighlight != null)
            {
                var pool = GetOrCreatePool(response.HighlightId);
                pool.Release(response.CurrentHighlight);
            }

            _highlightMappings.Remove(coordinate);
        }

        private BehaviourPool<HexHighlight> GetOrCreatePool(string highlightId)
        {
            if (!_highlightPools.TryGetValue(highlightId, out var pool))
            {
                var prefab = _highlightConfig.FindHighlightPrefab(highlightId);
                if (prefab == null)
                {
                    Log.Error($"Cannot find {highlightId}, your config messed up");
                    prefab = new GameObject("Error_HexHighlight").AddComponent<HexHighlight>();
                    // setting to inactive - this way at least we can hide the "prefab" from the active scene and the game looks a bit less jarring
                    prefab.gameObject.SetActive(false);
                }

                _highlightPools[highlightId] = pool = new BehaviourPool<HexHighlight>(prefab);
            }

            return pool;
        }

        private record ResponseRequest
        {
            public HighlightResponse? Response { get; set; }
            public SimplePriorityQueue<HighlightRequest> Requests { get; } = new();
        }

        private class HighlightRequest
        {
            public readonly string RequestId;
            public string HighlightId = string.Empty;

            public HighlightRequest(string requestId)
            {
                RequestId = requestId;
            }
        }

        private record HighlightResponse(string HighlightId, HexHighlight CurrentHighlight);
    }
}
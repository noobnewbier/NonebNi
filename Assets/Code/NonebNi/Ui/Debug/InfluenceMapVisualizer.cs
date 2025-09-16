using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.AI;
using NonebNi.Core.Coordinates;
using NonebNi.Core.DataIds;
using NonebNi.Core.Factions;
using NonebNi.Core.Maps;
using NonebNi.Terrain;
using UnityEngine;
using UnityUtils;
using UnityUtils.Pooling;
using Object = UnityEngine.Object;

namespace NonebNi.Ui.Debug
{
    public class InfluenceMapVisualizer : IDisposable
    {
        private readonly ICoordinateAndPositionService _coordinateAndPositionService;

        private readonly IFactionService _factionService;
        private readonly Texture2D _hexagonTex;

        private readonly Mesh _hexMesh;
        /*
         * Note:
         * Not using hex highlighter - it doesn't let us change color of a highlight request atm.
         * And changing it just because we need a debug tool seems awkward.
         *
         * It's easier to implement something new instead.
         */


        private readonly Dictionary<Coordinate, InfluenceHighlight> _highlightMappings = new();
        private readonly BehaviourPool<InfluenceHighlight> _influenceHighlightPool;
        private readonly IInfluenceMap _influenceMap;
        private readonly IReadOnlyMap _map;
        private readonly HashSet<DataId<Faction>> _visualizedFactions = new();

        public InfluenceMapVisualizer(
            TerrainConfigData terrainConfigData,
            ICoordinateAndPositionService coordinateAndPositionService,
            IReadOnlyMap map,
            InfluenceHighlight prefab,
            IFactionService factionService,
            IInfluenceMap influenceMap)
        {
            _coordinateAndPositionService = coordinateAndPositionService;
            _map = map;
            _factionService = factionService;
            _influenceMap = influenceMap;
            _influenceHighlightPool = new BehaviourPool<InfluenceHighlight>(prefab);

            #region Hack to get a hex mesh

            _hexagonTex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
            var pixels = _hexagonTex.GetPixels();
            var offset = -Vector2.one / 2f;
            var radius = 0.425f;
            for (var y = 0; y < _hexagonTex.height; y++)
            for (var x = 0; x < _hexagonTex.width; x++)
            {
                var point = new Vector2((float)x / _hexagonTex.width, (float)y / _hexagonTex.height);
                point += offset;
                var inSdf = SDFHexagon(point, radius) <= 0f;
                var color = inSdf ?
                    Color.white :
                    Color.clear;
                var index = y * _hexagonTex.width + x;
                pixels[index] = color;
            }

            _hexagonTex.SetPixels(pixels);
            _hexagonTex.Apply();

            /*
             * Copied from TerrainMeshCreator and "made it work" - if it works you lucked out.
             * Hacky way to get a hex mesh without us making a prefab risking it not matching what we need for the current terrain config
             *
             * The entire ctor is a hack as our DI framework can't have named registration, sometimes, life is tough I suppose.
             *
             * Note:
             * actually, can have [instance] field with serializable dictionary config, which would fix our issue.
             * we can even run another container for debug tools, but that means we lose access to rest of the game code...
             * or maybe I am just overthinking it - it's really not that bad is it, I mean it's quite bad but it won't be the first time (╯°□°）╯︵ ┻━┻
             */

            terrainConfigData = terrainConfigData with
            {
                // hacky way to not deal with duplicated registration on TerrainConfigData.
                SolidFactor = 1f
            };

            TerrainMeshData terrain = new();
            Triangulate(Coordinate.Zero);
            _hexMesh = terrain.Apply();

            void Triangulate(Coordinate cell)
            {
                var tempService = new CoordinateAndPositionService(terrainConfigData);
                foreach (var direction in HexDirection.All)
                {
                    var coordinatePos = tempService.FindPosition(cell);
                    var e = new EdgeVertices(
                        tempService.GetFirstSolidCorner(cell, direction),
                        tempService.GetSecondSolidCorner(cell, direction)
                    );

                    TriangulateEdgeFan(coordinatePos, e);
                }
            }

            void TriangulateEdgeFan(Vector3 center, EdgeVertices edge)
            {
                terrain.AddTriangle(center, edge.V1, edge.V2);
                terrain.AddTriangle(center, edge.V2, edge.V3);
                terrain.AddTriangle(center, edge.V3, edge.V4);
                terrain.AddTriangle(center, edge.V4, edge.V5);

                terrain.AddTriangleCellData(Vector3.one);
                terrain.AddTriangleCellData(Vector3.one);
                terrain.AddTriangleCellData(Vector3.one);
                terrain.AddTriangleCellData(Vector3.one);
            }

            float SDFHexagon(Vector2 p, in float r)
            {
                /*
                 * Note:
                 * SDF for 2D hexagons - helps generating the texture wihout us importing one.
                 * https://iquilezles.org/articles/distfunctions2d/
                 */
                var k = new Vector3(-0.866025404f, 0.5f, 0.577350269f);
                p = new Vector2(Mathf.Abs(p.x), Mathf.Abs(p.y));
                p -= 2f * Mathf.Min(Vector2.Dot(k.XY(), p), 0f) * k.XY();
                p -= new Vector2(Mathf.Clamp(p.x, -k.z * r, k.z * r), r);
                return p.magnitude * Mathf.Sign(p.y);
            }

            #endregion
        }

        public void Dispose()
        {
            Object.Destroy(_hexMesh);
        }


        public void SetEnable(bool isEnabled, IEnumerable<DataId<Faction>> factionIds)
        {
            if (isEnabled)
                _visualizedFactions.UnionWith(factionIds);
            else
                _visualizedFactions.ExceptWith(factionIds);

            RefreshHighlight();
        }

        private void RefreshHighlight()
        {
            var factions = _visualizedFactions.Select(i => _factionService.FindFaction(i)).ToArray();
            var maxInfluence = _visualizedFactions.Count;
            foreach (var coord in _map.GetAllCoordinates())
            {
                var highlight = GetOrCreateHighlight(coord);

                var validInfluences = factions
                    .Select(f => (faction: f, influence: _influenceMap.FindInfluence(f.Id, coord)))
                    .Where(t => t.influence > 0)
                    .ToArray();

                if (!validInfluences.Any())
                {
                    highlight.Draw(Color.clear, _hexagonTex);
                    continue;
                }

                var color = Color.black;
                foreach (var (faction, influence) in validInfluences)
                {
                    var weight = influence / maxInfluence;
                    var weightedColor = faction.FactionColor * weight;

                    color += weightedColor;
                }

                var alpha = validInfluences.Sum(t => t.influence) / maxInfluence;
                color.a = alpha;
                highlight.Draw(color, _hexagonTex);
            }
        }

        private InfluenceHighlight GetOrCreateHighlight(Coordinate coord)
        {
            if (!_highlightMappings.TryGetValue(coord, out var highlight))
            {
                _highlightMappings[coord] = highlight = _influenceHighlightPool.Get();
                highlight.transform.position = _coordinateAndPositionService.FindPosition(coord);
            }

            return highlight;
        }
    }
}
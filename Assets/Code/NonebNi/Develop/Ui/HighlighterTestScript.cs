using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using NonebNi.Terrain;
using NonebNi.Ui.Grids;
using UnityEngine;
using UnityEngine.InputSystem;
using static NonebNi.Develop.TestScriptHelpers;

namespace NonebNi.Develop
{
    public class HighlighterTestScript : MonoBehaviour
    {
        [SerializeField] private TerrainConfigData terrainConfig = null!;
        [SerializeField] private HexHighlightConfig highlightConfig = null!;
        private readonly List<Coordinate> _bulkingCoordinates = new();

        private HexHighlighter _highlighter = null!;
        private bool _isBulkMode;

        private bool _isInitialised;
        private bool _lastBulkInputIsActive;
        private IReadOnlyMap _map = null!;
        private string _requestId = "debug";
        private CoordinateAndPositionService _service = null!;
        private HighlightVariation _variation = HighlightVariation.Normal;

        private void Awake()
        {
            _isInitialised = true;

            _service = new CoordinateAndPositionService(terrainConfig);
            _highlighter = new HexHighlighter(_service, highlightConfig, terrainConfig);
            _map = new Map(10, 10);
        }

        private void Update()
        {
            ProcessInput();
        }

        private void OnGUI()
        {
            var startingRect = new Rect(10, 10, 150, 25);
            var rect = startingRect;
            if (!_isInitialised)
            {
                GUI.Label(rect, "Start the scene to test");
                return;
            }

            if (GUI.Button(rect, "Use TileInspection")) _requestId = HighlightRequestId.TileInspection;
            rect.y += 25;

            if (GUI.Button(rect, "Use TargetSelection")) _requestId = HighlightRequestId.TargetSelection;
            rect.y += 25;

            if (GUI.Button(rect, "Cycle Highlight"))
            {
                var allConfigs = highlightConfig.GetAll().ToArray();
                var index = Array.FindIndex(allConfigs, t => t.variation == _variation);

                index++;
                if (index >= allConfigs.Length) index = 0;

                _variation = allConfigs[index].variation;
            }

            rect.y += 25;

            // If this proves useful maybe make a helper to make this easier.
            if (GUI.Button(rect, "Clear All")) _highlighter.ClearAll();
            rect.y += 25;

            if (GUI.Button(rect, "Clear Id")) _highlighter.RemoveRequest(_requestId);
            rect.y += 25;

            _isBulkMode = GUI.Toggle(rect, _isBulkMode, "Bulk Mode");
            rect.y += 25;

            _requestId = GUI.TextField(rect, _requestId);
            rect.y += 25;

            if (GUI.Button(rect, _variation.ToString()))
            {
                var values = Enum.GetValues(typeof(HighlightVariation)).Cast<HighlightVariation>().ToList();
                var next = values.ElementAtOrDefault((int)_variation++);
                _variation = next;
            }

            rect.y += 25;

            GUI.Label(rect, "Left Click to Add");
            rect.y += 25;

            GUI.Label(rect, "Right Click to Remove");
            rect.y += 25;

            GUI.Box(new Rect(startingRect.x, startingRect.y, startingRect.width, rect.y - startingRect.y), string.Empty);
        }

        private void OnDrawGizmos()
        {
            if (!_isInitialised) return;

            DrawGridUsingGizmos(_service, _map);
        }

        private void ProcessInput()
        {
            var (success, pos) = FindMousePosInWorld(terrainConfig.GridPlane);
            if (!success) return;

            if (Mouse.current.leftButton.wasReleasedThisFrame) SetHighlight(pos, true);
            if (Mouse.current.rightButton.wasReleasedThisFrame) SetHighlight(pos, false);

            ProcessBulk();
        }

        private void ProcessBulk()
        {
            if (_isBulkMode) return;

            //Yes it's ugly but the purpose is to test the API not writing clean code.
            if (_bulkingCoordinates.Any())
            {
                if (_lastBulkInputIsActive)
                    _highlighter.RequestHighlight(_bulkingCoordinates, _requestId, _variation);
                else
                    _highlighter.RemoveRequest(_bulkingCoordinates, _requestId);

                _bulkingCoordinates.Clear();
            }
        }

        private void SetHighlight(Vector3 pos, bool isActive)
        {
            var coord = _service.NearestCoordinateForPoint(pos);
            if (!_map.IsCoordinateWithinMap(coord)) return;

            if (_isBulkMode)
            {
                _lastBulkInputIsActive = isActive;
                _bulkingCoordinates.Add(coord);
                return;
            }


            if (isActive)
                _highlighter.RequestHighlight(coord, _requestId, _variation);
            else
                _highlighter.RemoveRequest(coord, _requestId);
        }
    }
}
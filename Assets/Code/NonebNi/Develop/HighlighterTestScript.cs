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
        [SerializeField] private HexHighlight highlightPrefab = null!;
        private readonly List<Coordinate> _bulkingCoordinates = new();

        private HexHighlighter _highlighter = null!;
        private string _highlightId = "debug";
        private bool _isBulkMode;

        private bool _isInitialised;
        private bool _lastBulkInputIsActive;
        private IReadOnlyMap _map = null!;
        private CoordinateAndPositionService _service = null!;

        private void Awake()
        {
            _isInitialised = true;

            _service = new CoordinateAndPositionService(terrainConfig);
            _highlighter = new HexHighlighter(highlightPrefab, _service);
            _map = new Map(40, 40);
        }

        private void Update()
        {
            ProcessInput();
        }

        private void OnGUI()
        {
            var rect = new Rect(10, 10, 150, 25);
            if (!_isInitialised)
            {
                GUI.Label(rect, "Start the scene to test");
                return;
            }

            // If this proves useful maybe make a helper to make this easier.
            if (GUI.Button(rect, "Clear All")) _highlighter.ClearAll();
            rect.y += 25;

            if (GUI.Button(rect, "Clear Id")) _highlighter.RemoveHighlight(_highlightId);
            rect.y += 25;

            _isBulkMode = GUI.Toggle(rect, _isBulkMode, "Bulk Mode");
            rect.y += 25;

            _highlightId = GUI.TextField(rect, _highlightId);
            rect.y += 25;

            GUI.Label(rect, "Left Click to Add");
            rect.y += 25;

            GUI.Label(rect, "Right Click to Remove");
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
                    _highlighter.AddHighlight(_bulkingCoordinates, _highlightId);
                else
                    _highlighter.RemoveHighlight(_bulkingCoordinates, _highlightId);

                _bulkingCoordinates.Clear();
            }
        }

        private void SetHighlight(Vector3 pos, bool isActive)
        {
            var coord = _service.NearestCoordinateForPoint(pos);
            if (_isBulkMode)
            {
                _lastBulkInputIsActive = isActive;
                _bulkingCoordinates.Add(coord);
                return;
            }


            if (isActive)
                _highlighter.AddHighlight(coord, _highlightId);
            else
                _highlighter.RemoveHighlight(coord, _highlightId);
        }
    }
}
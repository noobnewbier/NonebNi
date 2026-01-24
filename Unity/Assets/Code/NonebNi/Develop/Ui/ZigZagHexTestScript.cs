using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using NonebNi.Terrain;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtils.Editor;
using static NonebNi.Develop.TestScriptHelpers;

namespace NonebNi.Develop
{
    public class ZigZagHexTestScript : MonoBehaviour
    {
        private readonly TerrainConfigData _configData = TerrainConfigData.Default;
        private int _distToCheck;

        private bool _isInitialised;
        private IReadOnlyMap _map = null!;

        private Coordinate _pointOrigin = Coordinate.Zero;
        private CoordinateAndPositionService _service = null!;

        private void Awake()
        {
            _isInitialised = true;

            _service = new (_configData);
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
                GUI.Label(rect, "Start the scene to test - white is most zigzag");
                return;
            }

            GUI.Label(rect, "Left Click to change origin - white is most zigzag");
            rect.y += 25;

            _distToCheck = (int)GUI.HorizontalSlider(rect, _distToCheck, 1, 10);
            rect.y += 25;
        }

        private void OnDrawGizmos()
        {
            if (!_isInitialised) return;

            DrawGridUsingGizmos(_service, _map);

            var aPos = _service.FindPosition(_pointOrigin);

            using (new NonebEditorGUI.GizmosColorScope(Color.green))
            {
                Gizmos.DrawSphere(aPos, 0.25f);
            }

            foreach (var coord in _pointOrigin.WithinDistance(_distToCheck))
            {
                var zigzagness = _pointOrigin.ZigZagnessWithinRange(coord);
                var color = Color.Lerp(Color.black, Color.white, zigzagness);

                using (new NonebEditorGUI.GizmosColorScope(color))
                {
                    var pos = _service.FindPosition(coord);
                    Gizmos.DrawSphere(pos, 0.5f);
                }
            }
        }

        private void ProcessInput()
        {
            var (success, pos) = FindMousePosInWorld(_configData.GridPlane);
            if (!success) return;

            if (Mouse.current.leftButton.wasReleasedThisFrame) _pointOrigin = _service.NearestCoordinateForPoint(pos);
        }
    }
}
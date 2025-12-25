using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using NonebNi.Terrain;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtils.Editor;
using static NonebNi.Develop.TestScriptHelpers;

namespace NonebNi.Develop
{
    public class LineHexesTestScript : MonoBehaviour
    {
        private readonly TerrainConfigData _configData = TerrainConfigData.Default;

        private bool _isInitialised;
        private IReadOnlyMap _map = null!;

        private Coordinate _pointA = Coordinate.Zero;
        private Coordinate _pointB = Coordinate.Zero;
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
                GUI.Label(rect, "Start the scene to test");
                return;
            }

            GUI.Label(rect, "Left Click for Point A");
            rect.y += 25;

            GUI.Label(rect, "Right Click for Point B");
            rect.y += 25;
        }

        private void OnDrawGizmos()
        {
            if (!_isInitialised) return;

            DrawGridUsingGizmos(_service, _map);

            var aPos = _service.FindPosition(_pointA);
            var bPos = _service.FindPosition(_pointB);

            using (new NonebEditorGUI.GizmosColorScope(Color.green))
            {
                Gizmos.DrawSphere(aPos, 0.25f);
                Gizmos.DrawSphere(bPos, 0.25f);
            }

            using (new NonebEditorGUI.GizmosColorScope(Color.yellow))
            {
                Gizmos.DrawLine(aPos, bPos);
            }

            using (new NonebEditorGUI.GizmosColorScope(Color.red))
            {
                foreach (var coord in _pointA.GetCoordinatesBetween(_pointB))
                {
                    var inBetween = _service.FindPosition(coord);
                    Gizmos.DrawSphere(inBetween, 0.5f);
                }
            }
        }

        private void ProcessInput()
        {
            var (success, pos) = FindMousePosInWorld(_configData.GridPlane);
            if (!success) return;

            if (Mouse.current.leftButton.wasReleasedThisFrame) _pointA = _service.NearestCoordinateForPoint(pos);
            if (Mouse.current.rightButton.wasReleasedThisFrame) _pointB = _service.NearestCoordinateForPoint(pos);
        }
    }
}
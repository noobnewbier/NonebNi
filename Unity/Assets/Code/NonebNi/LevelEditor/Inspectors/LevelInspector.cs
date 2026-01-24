using System;
using Noneb.Logs.Runtime;
using NonebNi.Core.Diagnostics;
using NonebNi.LevelEditor.Level.Maps;
using NonebNi.LevelEditor.Level.Tiles;
using NonebNi.Terrain;
using UnityEditor;
using UnityEngine;
using UnityUtils;

namespace NonebNi.LevelEditor.Inspectors
{
    public class LevelInspector : IDisposable
    {
        private readonly ICoordinateAndPositionService _coordinateAndPositionService;
        private readonly GridView _gridView;
        private readonly TileInspectorView _tileInspectorView;

        public LevelInspector(GridView gridView, TileInspectorView tileInspectorView, ICoordinateAndPositionService coordinateAndPositionService)
        {
            _gridView = gridView;
            _tileInspectorView = tileInspectorView;
            _coordinateAndPositionService = coordinateAndPositionService;

            SceneView.duringSceneGui += OnSceneGUI;
            Diagnostic.DRequestCreated += OnDRequestCreated;
        }

        public void Dispose()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            Diagnostic.DRequestCreated -= OnDRequestCreated;
        }

        private void OnDRequestCreated(DRequest request)
        {
            // switch to dynamic dispatch if this gets too big.
            switch (request)
            {
                case DRequest.EngageUtility engageUtility:
                    var position = _coordinateAndPositionService.FindPosition(engageUtility.Coordinate);
                    var label = $@"{engageUtility.Coordinate}
Dist: {engageUtility.DistUtil}
OutOfLine: -{engageUtility.OutOfLinePenalty}
CrossLane: -{engageUtility.CrossLanePenalty}
Total: {engageUtility.DistUtil - engageUtility.OutOfLinePenalty - engageUtility.CrossLanePenalty}";

                    GizmosDrawer.DrawDynamicLabel(position, label, Color.black, 1f);
                    break;

                default:
                    Log.Warn("Editor", "Unhandled DRequest - diagnostic missing in the scene");
                    break;
            }
        }

        private void OnSceneGUI(SceneView view)
        {
            _gridView.OnSceneDraw();

            var sceneViewSize = SceneView.lastActiveSceneView.position.size;
            var position = new Vector2
            (
                0,
                sceneViewSize.y - TileInspectorView.WindowSize.y - SceneViewConstants.PaddingFromBottom
            );

            _tileInspectorView.OnSceneDraw(position);
        }
    }
}
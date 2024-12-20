using System;
using NonebNi.LevelEditor.Level.Maps;
using NonebNi.LevelEditor.Level.Tiles;
using UnityEditor;
using UnityEngine;

namespace NonebNi.LevelEditor.Inspectors
{
    public class LevelInspector : IDisposable
    {
        private readonly GridView _gridView;
        private readonly TileInspectorView _tileInspectorView;

        public LevelInspector(GridView gridView, TileInspectorView tileInspectorView)
        {
            _gridView = gridView;
            _tileInspectorView = tileInspectorView;

            SceneView.duringSceneGui += OnSceneGUI;
        }

        public void Dispose()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView view)
        {
            _gridView.OnSceneDraw();

            var sceneViewSize = SceneView.lastActiveSceneView.position.size;
            var position = new Vector2(
                0,
                sceneViewSize.y - TileInspectorView.WindowSize.y - SceneViewConstants.PaddingFromBottom
            );

            _tileInspectorView.OnSceneDraw(position);
        }
    }
}
﻿using NonebNi.Terrain;
using UnityEngine;

namespace NonebNi.Ui.Grids
{
    /// <summary>
    /// Uses line renderer to draw some lines, but in the future it will be more beautiful(hopefully)
    /// </summary>
    public class HexHighlight : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer = null!;

        private float _hexInnerRadius = -1f;

        private void Awake()
        {
            if (lineRenderer == null) lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        public void Draw(float targetHexInnerRadius)
        {
            if (Mathf.Approximately(_hexInnerRadius, targetHexInnerRadius)) return;

            _hexInnerRadius = targetHexInnerRadius;
            RedrawHexLines();
        }

        private void RedrawHexLines()
        {
            var corners = HexMaths.FindCorners(_hexInnerRadius);
            lineRenderer.positionCount = corners.Length;
            lineRenderer.SetPositions(corners);
        }
    }
}
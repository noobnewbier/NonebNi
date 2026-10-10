using System;
using NonebNi.Art.Pixelation;
using UnityEngine;

namespace Code
{
    public class SnapToPixelGrid : MonoBehaviour
    {
        [SerializeField] private PixelAlignController pixelAlignController;
        
        private Vector3 _cachedPosition;

        private void OnEnable()
        {
            pixelAlignController.SnapRequested += SnapToTexelPos;
            pixelAlignController.UnsnapRequested += RestorePosition;
        }

        private void OnDisable()
        {
            pixelAlignController.SnapRequested -= SnapToTexelPos;
            pixelAlignController.UnsnapRequested -= RestorePosition;
        }

        private void SnapToTexelPos()
        {
            _cachedPosition = transform.position;

            (Vector3 texelPos, Vector3 _) = pixelAlignController.FindClosestTexelPos(transform.position);
            transform.position = texelPos;
        }

        private void RestorePosition()
        {
            transform.position = _cachedPosition;
        }
    }
}

using System.Linq;
using NonebNi.Core.Maps;
using NonebNi.Terrain;
using UnityEngine;

namespace NonebNi.Ui.Cameras
{
    public interface ICameraControllerPresenter
    {
        void OnPanning(Vector3 panningDirection, float panningStrength, float deltaTime);
        void OnZooming(float zoomingStrength, float deltaTime);
    }

    public class CameraControllerPresenter : ICameraControllerPresenter
    {
        private readonly Vector2 _cameraViewSize;
        private readonly CameraConfig _config;
        private readonly float _downBound;
        private readonly float _leftBound;
        private readonly Vector2 _mapSize;
        private readonly float _minCameraY;
        private readonly float _rightBound;

        private readonly float _upBound;
        private readonly ICameraControllerView _view;

        public CameraControllerPresenter(CameraConfig config,
            IReadOnlyMap map,
            TerrainConfigData terrainConfigData,
            ICoordinateAndPositionService coordinateAndPositionService,
            ICameraControllerView view)
        {
            _config = config;
            _view = view;

            #region Calculate Camera Related Metrics

            var coordinatePositions = map.GetAllCoordinates().Select(coordinateAndPositionService.FindPosition).ToArray();

            var (minWidth, distanceToTop, distanceToBottom) = _view.GetViewDistanceToFrustumOnPlaneInWorldSpace();
            var mapMaxHeight = coordinatePositions.DefaultIfEmpty().Max(p => p.z);
            var mapMinHeight = coordinatePositions.DefaultIfEmpty().Min(p => p.z);
            var mapMaxWidth = coordinatePositions.DefaultIfEmpty().Max(p => p.x);
            var mapMinWidth = coordinatePositions.DefaultIfEmpty().Min(p => p.x);

            _upBound = mapMaxHeight + _config.BufferToClampingEdge - distanceToTop / 2f;
            _downBound = mapMinHeight - _config.BufferToClampingEdge + distanceToBottom / 2f;
            _rightBound = mapMaxWidth + _config.BufferToClampingEdge - minWidth / 2f;
            _leftBound = mapMinWidth - _config.BufferToClampingEdge + minWidth / 2f;

            _upBound = Mathf.Max(_upBound, _downBound);
            _rightBound = Mathf.Max(_rightBound, _leftBound);

            _cameraViewSize = new Vector2(minWidth, distanceToTop + distanceToBottom);
            _mapSize = new Vector2(mapMaxWidth - mapMinWidth, mapMaxHeight - mapMinHeight);
            _minCameraY = _config.MinDistanceToMap + terrainConfigData.MapStartingPosition.y;

            #endregion
        }

        public void OnPanning(Vector3 panningDirection, float panningStrength, float deltaTime)
        {
            var currentCameraPosition = _view.GetCameraPosition();
            var newCameraPosition = currentCameraPosition;

            newCameraPosition += panningDirection * (_config.MaxPanningSpeed * deltaTime * panningStrength);

            newCameraPosition = GetClampedCameraPosition(newCameraPosition);

            _view.MoveCameraTo(newCameraPosition);
        }

        //negative zooming strength for zooming in, positive for zooming out
        public void OnZooming(float zoomingStrength, float deltaTime)
        {
            if (zoomingStrength > 0f)
                if (_cameraViewSize.x > _mapSize.x && _cameraViewSize.y > _mapSize.y && _cameraViewSize.y > _minCameraY)
                    //don't zoom out too far
                    return;

            var currentPosition = _view.GetCameraPosition();
            var newYPosition = currentPosition.y + zoomingStrength * deltaTime * _config.MaxZoomingSpeed;
            newYPosition = Mathf.Max(newYPosition, _minCameraY);

            _view.MoveCameraTo(new Vector3(currentPosition.x, newYPosition, currentPosition.z));
        }

        private Vector3 GetClampedCameraPosition(Vector3 unclampedPosition)
        {
            unclampedPosition.x = Mathf.Clamp(unclampedPosition.x, _leftBound, _rightBound);
            unclampedPosition.z = Mathf.Clamp(unclampedPosition.z, _downBound, _upBound);
            unclampedPosition.y = Mathf.Max(unclampedPosition.y, _minCameraY);

            return unclampedPosition;
        }
    }
}
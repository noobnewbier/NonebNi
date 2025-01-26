using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtils;

namespace NonebNi.Ui.Cameras
{
    public interface ICameraController
    {
        void LookAt(Vector3 position);
        void UpdateCamera();
    }

    /// <summary>
    /// Note:
    /// If we want to do a more in depth camera control, so like transitioning between camera mode, zoom in on combo cinematics
    /// etc,
    /// we would need a to have another layer of "camera control" code in the lower level, and only have this to control that.
    /// That "lower level" thing can be implemented via a mix of cinemachine stuffs?
    /// </summary>
    public class CameraController : ICameraController
    {
        private readonly CinemachineCamera _camera;
        private readonly CameraConfig _config;
        private readonly CinemachinePositionComposer _positionComposer;

        private float _accumulatedZoomingDecelerationValue;
        private float _accumulatedZoomingValue;
        private float _currentZoomingDirection;

        public CameraController(
            CameraConfig config,
            CinemachineCamera controlledCamera,
            CinemachinePositionComposer positionComposer)
        {
            _config = config;
            _camera = controlledCamera;
            _positionComposer = positionComposer;
        }

        private Vector3 TargetPos
        {
            get => _camera.Target.TrackingTarget.transform.position;
            set => _camera.Target.TrackingTarget.transform.position = value;
        }

        private float DownBound => Mathf.Sign(_config.UpBound) * _config.Setting.BufferToClampingEdge;
        private float UpBound => Mathf.Sign(_config.DownBound) * _config.Setting.BufferToClampingEdge;
        private float RightBound => Mathf.Sign(_config.RightBound) * _config.Setting.BufferToClampingEdge;
        private float LeftBound => Mathf.Sign(_config.LeftBound) * _config.Setting.BufferToClampingEdge;

        public void LookAt(Vector3 position)
        {
            //TODO: if too far away - turn off smoothing and look ahead.
            TargetPos = position;
        }

        public void UpdateCamera()
        {
            Panning();
            Zooming();
        }

        private void Pan(Vector3 panningDirection, float panningStrength, float deltaTime)
        {
            var currentTargetPosition = TargetPos;
            var newTargetPosition = currentTargetPosition;

            newTargetPosition += panningDirection * (_config.Setting.MaxPanningSpeed * deltaTime * panningStrength);

            // Clamping
            newTargetPosition.x = Mathf.Clamp(newTargetPosition.x, LeftBound, RightBound);
            newTargetPosition.z = Mathf.Clamp(newTargetPosition.z, DownBound, UpBound);
            newTargetPosition.y = TargetPos.y; //touching this might means you want to change the zoom but this method not what it's meant for

            TargetPos = newTargetPosition;
        }

        //negative zooming strength for zooming in, positive for zooming out
        private void Zoom(float zoomingStrength, float deltaTime)
        {
            var currentZoom = _positionComposer.CameraDistance;
            var newZoom = currentZoom + zoomingStrength * deltaTime * _config.Setting.MaxZoomingSpeed;
            newZoom = Mathf.Max(newZoom, _config.Setting.MinDistanceToMap);
            newZoom = Mathf.Min(newZoom, _config.Setting.MaxDistanceToMap);

            _positionComposer.CameraDistance = newZoom;
        }

        #region Panning

        private void Panning() //consider adding panning with middle mouse button
        {
            //TODO: include look ahead in your composer as it feels okay?

            var panningStrength = GetPanningStrength();
            if (panningStrength.NearlyEqual(0f)) return;

            var mousePosition = Input.mousePosition;

            var panningDirection = (mousePosition - new Vector3(Screen.width / 2f, Screen.height / 2f, 0)).normalized;
            panningDirection.z = panningDirection.y;
            panningDirection.y = 0;

            Pan(panningDirection, GetPanningStrength(), Time.deltaTime);
        }

        private float GetPanningStrength()
        {
            var mousePosition = Input.mousePosition;
            var yDistancePercentage = 1f -
                                      Mathf.Min(
                                          mousePosition.y,                //to bottom
                                          Screen.height - mousePosition.y //to top
                                      ) /
                                      (Screen.height / 2f);
            var xDistancePercentage = 1f -
                                      Mathf.Min(
                                          mousePosition.x,               //to left
                                          Screen.width - mousePosition.x //to right
                                      ) /
                                      (Screen.width / 2f);


            if (yDistancePercentage < _config.Setting.EdgePercentageToPan && xDistancePercentage < _config.Setting.EdgePercentageToPan ||
                yDistancePercentage > 1f ||
                xDistancePercentage > 1f) // prevent panning when mouse is outside of windows
                return 0;

            var center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            var mouseDistanceFromCenter = Vector3.Distance(mousePosition, center);
            var distanceInPercentage = mouseDistanceFromCenter / center.magnitude;
            var thresholdDistanceInPercentage = center.magnitude * _config.Setting.EdgePercentageToPan;
            return Easing.ExponentialEaseInOut(
                Mathf.Abs(distanceInPercentage - thresholdDistanceInPercentage / (1f - thresholdDistanceInPercentage)),
                _config.Setting.SmoothFactor
            );
        }

        #endregion

        #region Zooming

        private void Zooming()
        {
            var zoomingStrength = GetZoomingStrength();
            if (zoomingStrength.NearlyEqual(0f)) return;

            Zoom(zoomingStrength, Time.deltaTime);
        }


        private float GetZoomingStrength()
        {
            //TODO: separate input and the movement code...?
            //TODO: was using old input system - when the time comes we need our own input wrapper.
            var zoomInput = Mouse.current.scroll.ReadValue().normalized.y;

            var inputStrength = Mathf.Abs(zoomInput);
            if (!zoomInput.NearlyEqual(0f))
                _currentZoomingDirection = Mathf.Sign(zoomInput) * (_config.Setting.IsInvertedWheel ?
                    -1f :
                    1f);

            if (!zoomInput.NearlyEqual(0f))
            {
                //accelerating
                _accumulatedZoomingDecelerationValue = 0;
                _accumulatedZoomingValue += inputStrength;
                _accumulatedZoomingValue = Mathf.Clamp01(_accumulatedZoomingValue);
            }
            else if (!_accumulatedZoomingValue.NearlyEqual(0f))
            {
                //decelerating

                _accumulatedZoomingDecelerationValue += _config.Setting.DecelerationSpeed * Time.deltaTime;
                _accumulatedZoomingDecelerationValue = Mathf.Clamp01(_accumulatedZoomingDecelerationValue);

                _accumulatedZoomingValue -= Easing.ExponentialEaseInOut(
                    _accumulatedZoomingDecelerationValue,
                    _config.Setting.SmoothFactor
                );
                _accumulatedZoomingValue = Mathf.Clamp01(_accumulatedZoomingValue);
            }
            else
            {
                //all is set, do nothing
                _currentZoomingDirection = 0f;
                return 0;
            }

            return _currentZoomingDirection * Easing.ExponentialEaseInOut(_accumulatedZoomingValue, _config.Setting.SmoothFactor);
        }

        #endregion
    }
}
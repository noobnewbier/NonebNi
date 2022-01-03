using System.Linq;
using NonebNi.Core.Level;
using UnityEngine;
using UnityUtils;
using UnityUtils.Factories;

namespace NonebNi.Ui.Cameras
{
    public interface ICameraControllerView
    {
        /// <summary>
        /// Finding 4 intersection point of controlledCamera frustum on the infinite plane(at mapTransform's height).
        /// Returning the maximum size of a rect that it can be bounded within the 4 intersection point.
        /// </summary>
        (float minWidth, float distanceToTop, float distanceToBottom) GetViewDistanceToFrustumOnPlaneInWorldSpace();

        void MoveCameraTo(Vector3 position);

        Vector3 GetCameraPosition();
        void UpdateCamera();
    }

    public class CameraControllerView : ICameraControllerView
    {
        private readonly Camera _camera;
        private readonly CameraConfig _config;
        private readonly ICameraControllerPresenter _controllerPresenter;
        private readonly WorldConfigData _worldConfigData;

        private float _accumulatedZoomingDecelerationValue;
        private float _accumulatedZoomingValue;
        private float _currentZoomingDirection;

        public CameraControllerView(CameraConfig config,
                                    Camera controlledCamera,
                                    WorldConfigData worldConfigData,
                                    IFactory<ICameraControllerView, ICameraControllerPresenter> presenterFactory)
        {
            _config = config;
            _camera = controlledCamera;
            _worldConfigData = worldConfigData;
            _controllerPresenter = presenterFactory.Create(this);
        }

        public void MoveCameraTo(Vector3 position)
        {
            _camera.transform.position = position;
        }

        public Vector3 GetCameraPosition() => _camera.transform.position;

        public void UpdateCamera()
        {
            Panning();
            Zooming();
        }

        #region Panning

        private void Panning() //consider adding panning with middle mouse button
        {
            var panningStrength = GetPanningStrength();
            if (FloatUtil.NearlyEqual(panningStrength, 0f)) return;

            var mousePosition = Input.mousePosition;

            var panningDirection = (mousePosition - new Vector3(Screen.width / 2f, Screen.height / 2f, 0)).normalized;
            panningDirection.z = panningDirection.y;
            panningDirection.y = 0;

            _controllerPresenter.OnPanning(panningDirection, GetPanningStrength(), Time.deltaTime);
        }

        private float GetPanningStrength()
        {
            var mousePosition = Input.mousePosition;
            var yDistancePercentage = 1f -
                                      Mathf.Min(
                                          mousePosition.y, //to bottom
                                          Screen.height - mousePosition.y //to top
                                      ) /
                                      (Screen.height / 2f);
            var xDistancePercentage = 1f -
                                      Mathf.Min(
                                          mousePosition.x, //to left
                                          Screen.width - mousePosition.x //to right
                                      ) /
                                      (Screen.width / 2f);


            if (yDistancePercentage < _config.EdgePercentageToPan && xDistancePercentage < _config.EdgePercentageToPan ||
                yDistancePercentage > 1f ||
                xDistancePercentage > 1f) // prevent panning when mouse is outside of windows
                return 0;

            var center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            var mouseDistanceFromCenter = Vector3.Distance(mousePosition, center);
            var distanceInPercentage = mouseDistanceFromCenter / center.magnitude;
            var thresholdDistanceInPercentage = center.magnitude * _config.EdgePercentageToPan;
            return Easing.ExponentialEaseInOut(
                Mathf.Abs(distanceInPercentage - thresholdDistanceInPercentage / (1f - thresholdDistanceInPercentage)),
                _config.SmoothFactor
            );
        }

        #endregion

        #region Zooming

        private void Zooming()
        {
            var zoomingStrength = GetZoomingStrength();
            if (FloatUtil.NearlyEqual(zoomingStrength, 0f)) return;

            _controllerPresenter.OnZooming(zoomingStrength, Time.deltaTime);
        }


        private float GetZoomingStrength()
        {
            var zoomInput = Input.GetAxis("Mouse ScrollWheel");
            var inputStrength = Mathf.Abs(zoomInput);
            if (!FloatUtil.NearlyEqual(zoomInput, 0f))
                _currentZoomingDirection = Mathf.Sign(zoomInput) * (_config.IsInvertedWheel ? -1f : 1f);

            if (!FloatUtil.NearlyEqual(zoomInput, 0f))
            {
                //accelerating
                _accumulatedZoomingDecelerationValue = 0;
                _accumulatedZoomingValue += inputStrength;
                _accumulatedZoomingValue = Mathf.Clamp01(_accumulatedZoomingValue);
            }
            else if (!FloatUtil.NearlyEqual(_accumulatedZoomingValue, 0f))
            {
                //decelerating

                _accumulatedZoomingDecelerationValue += _config.DecelerationSpeed * Time.deltaTime;
                _accumulatedZoomingDecelerationValue = Mathf.Clamp01(_accumulatedZoomingDecelerationValue);

                _accumulatedZoomingValue -= Easing.ExponentialEaseInOut(
                    _accumulatedZoomingDecelerationValue,
                    _config.SmoothFactor
                );
                _accumulatedZoomingValue = Mathf.Clamp01(_accumulatedZoomingValue);
            }
            else
            {
                //all is set, do nothing
                _currentZoomingDirection = 0f;
                return 0;
            }

            return _currentZoomingDirection * Easing.ExponentialEaseInOut(_accumulatedZoomingValue, _config.SmoothFactor);
        }

        #endregion

        #region Camera Bounds

        /// <summary>
        /// Finding 4 intersection point of controlledCamera frustum on the infinite plane(at mapTransform's height).
        /// Returning the maximum size of a rect that it can be bounded within the 4 intersection point.
        /// </summary>
        public (float minWidth, float distanceToTop, float distanceToBottom) GetViewDistanceToFrustumOnPlaneInWorldSpace()
        {
            var cameraFrustumCorners = GetCameraFrustumCorners();
            var cameraPosition = GetCameraPosition();
            var targetYPosition = _worldConfigData.MapStartingPosition.y;
            var intersectionCorners = new Vector3[cameraFrustumCorners.Length];

            for (var i = 0; i < cameraFrustumCorners.Length; i++)
            {
                var (x, z) = LinearEquations.GetXzGivenY(targetYPosition, cameraPosition, cameraFrustumCorners[i]);
                intersectionCorners[i] = new Vector3(x, targetYPosition, z);
            }

            var distanceToTop = intersectionCorners[1].z - cameraPosition.z;
            var distanceToBottom = cameraPosition.z - intersectionCorners[0].z;
            var minWidth = intersectionCorners[3].x - intersectionCorners[0].x;

            return (minWidth, distanceToTop, distanceToBottom);
        }

        private Vector3[] GetCameraFrustumCorners()
        {
            var cameraTransform = _camera.transform;
            var frustumCorners = new Vector3[4];
            _camera.CalculateFrustumCorners(
                new Rect(
                    0,
                    0,
                    1,
                    1
                ),
                _camera.farClipPlane,
                Camera.MonoOrStereoscopicEye.Mono,
                frustumCorners
            );

            return frustumCorners.Select(c => cameraTransform.TransformVector(c) + cameraTransform.position).ToArray();
        }

        #endregion
    }
}
using NonebNi.Ui.Cameras;
using UnityEngine;

namespace NonebNi.Main
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private CameraConfig config = null!;
        [SerializeField] private Camera targetCamera = null!;

        private ICameraControllerView _cameraControllerView = null!;

        private bool _isRunning;

        public CameraConfig Config => config;

        public Camera TargetCamera => targetCamera;

        private void Update()
        {
            if (_isRunning) _cameraControllerView.UpdateCamera();
        }

        public void Init(ICameraControllerView cameraControllerView)
        {
            _cameraControllerView = cameraControllerView;
        }

        public void Run()
        {
            _isRunning = true;
        }
    }
}
using NonebNi.Main.Di;
using NonebNi.Ui.Cameras;
using UnityEngine;

namespace NonebNi.Main
{
    public class CameraControl : MonoBehaviour
    {
        [SerializeField] private CameraConfig config = null!;
        [SerializeField] private Camera targetCamera = null!;

        private ICameraControllerView _cameraControllerView = null!;

        private bool _initialized;

        private void Update()
        {
            if (_initialized) _cameraControllerView.UpdateCamera();
        }

        public void Init(ILevelComponent levelComponent,
                         CoordinateAndPositionServiceModule coordinateAndPositionServiceModule)
        {
            _initialized = true;

            var module = new CameraControlModule(config, targetCamera);
            var component = new CameraControlComponent(module, levelComponent, coordinateAndPositionServiceModule);
            _cameraControllerView = component.CreateCameraControllerView();
        }
    }
}
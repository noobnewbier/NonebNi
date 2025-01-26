using NonebNi.Ui.Cameras;
using Unity.Cinemachine;
using UnityEngine;

namespace NonebNi.Main
{
    public class CameraRunner : MonoBehaviour
    {
        [field: SerializeField] public CameraControlSetting Config { get; private set; } = null!;
        [field: SerializeField] public CinemachineCamera CinemachineCamera { get; private set; } = null!;
        [field: SerializeField] public CinemachinePositionComposer Composer { get; private set; } = null!;

        private ICameraController _controller = null!;
        private bool _isRunning;

        public void Init(ICameraController cameraControllerView)
        {
            _controller = cameraControllerView;
        }

        private void Update()
        {
            if (_isRunning) _controller.UpdateCamera();
        }

        public void Run()
        {
            _isRunning = true;
        }
    }
}
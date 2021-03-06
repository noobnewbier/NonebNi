using NonebNi.Ui.Cameras;
using UnityEngine;

namespace NonebNi.Main.Di
{
    public class CameraControlModule
    {
        public CameraConfig CameraConfig { get; }

        public Camera Camera { get; }

        public CameraControlModule(CameraConfig cameraConfig, Camera camera)
        {
            CameraConfig = cameraConfig;
            Camera = camera;
        }
    }
}
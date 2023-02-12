using NonebNi.Core.Coordinates;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Ui.Cameras;
using StrongInject;
using UnityEngine;
using UnityUtils.Factories;

namespace NonebNi.Main.Di
{
    public class CameraControlViewModule
    {
        [Factory]
        public static ICameraControllerView CreateCameraControllerView(IReadOnlyMap map,
            WorldConfigData worldConfig,
            ICoordinateAndPositionService coordinateAndPositionService,
            CameraConfig cameraConfig,
            Camera camera)
        {
            var presenterFactory = Factory.Create<ICameraControllerView, ICameraControllerPresenter>(
                v => new CameraControllerPresenter(
                    cameraConfig,
                    map,
                    worldConfig,
                    coordinateAndPositionService,
                    v
                )
            );

            return new CameraControllerView(
                cameraConfig,
                camera,
                worldConfig,
                presenterFactory
            );
        }
    }
}
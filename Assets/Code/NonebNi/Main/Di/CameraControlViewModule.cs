using NonebNi.Core.Maps;
using NonebNi.Terrain;
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
            TerrainConfigData terrainConfig,
            ICoordinateAndPositionService coordinateAndPositionService,
            CameraConfig cameraConfig,
            Camera camera)
        {
            var presenterFactory = Factory.Create<ICameraControllerView, ICameraControllerPresenter>(
                v => new CameraControllerPresenter(
                    cameraConfig,
                    map,
                    terrainConfig,
                    coordinateAndPositionService,
                    v
                )
            );

            return new CameraControllerView(
                cameraConfig,
                camera,
                terrainConfig,
                presenterFactory
            );
        }
    }
}
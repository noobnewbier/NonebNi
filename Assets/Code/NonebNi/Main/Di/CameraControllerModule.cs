using System.Linq;
using NonebNi.Core.Maps;
using NonebNi.Terrain;
using NonebNi.Ui.Cameras;
using StrongInject;
using Unity.Cinemachine;

namespace NonebNi.Main.Di
{
    public class CameraControllerModule
    {
        [Factory]
        public static CameraConfig CreateCameraConfig(CameraControlSetting setting, ICoordinateAndPositionService coordinateAndPositionService, IReadOnlyMap map)
        {
            var coordinatePositions = map.GetAllCoordinates().Select(coordinateAndPositionService.FindPosition).ToArray();
            var mapMaxHeight = coordinatePositions.DefaultIfEmpty().Max(p => p.z);
            var mapMinHeight = coordinatePositions.DefaultIfEmpty().Min(p => p.z);
            var mapMaxWidth = coordinatePositions.DefaultIfEmpty().Max(p => p.x);
            var mapMinWidth = coordinatePositions.DefaultIfEmpty().Min(p => p.x);

            return new CameraConfig(setting, mapMinHeight, mapMinWidth, mapMaxWidth, mapMaxHeight);
        }

        [Factory]
        public static ICameraController CreateCameraControllerView(CameraConfig config, CinemachineCamera camera, CinemachinePositionComposer composer) => new CameraController(config, camera, composer);
    }
}
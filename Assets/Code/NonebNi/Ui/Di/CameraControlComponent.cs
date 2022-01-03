using NonebNi.Core.Di;
using NonebNi.Ui.Cameras;
using UnityUtils.Factories;

namespace NonebNi.Ui.Di
{
    public interface ICameraControlComponent
    {
        ICameraControllerView CreateCameraControllerView();
    }

    public class CameraControlComponent : ICameraControlComponent
    {
        private readonly CameraControlModule _cameraModule;
        private readonly CoordinateAndPositionServiceModule _coordinateAndPositionServiceModule;
        private readonly ILevelComponent _levelComponent;


        public CameraControlComponent(CameraControlModule cameraModule,
                                      ILevelComponent levelComponent,
                                      CoordinateAndPositionServiceModule coordinateAndPositionServiceModule)
        {
            _cameraModule = cameraModule;
            _levelComponent = levelComponent;
            _coordinateAndPositionServiceModule = coordinateAndPositionServiceModule;
        }

        public ICameraControllerView CreateCameraControllerView()
        {
            var levelData = _levelComponent.GetLevelData();
            var presenterFactory = Factory.Create<ICameraControllerView, ICameraControllerPresenter>(
                v => new CameraControllerPresenter(
                    _cameraModule.CameraConfig,
                    levelData.Map,
                    levelData.WorldConfig,
                    _coordinateAndPositionServiceModule.GetCoordinateAndPositionService(),
                    v
                )
            );

            return new CameraControllerView(
                _cameraModule.CameraConfig,
                _cameraModule.Camera,
                levelData.WorldConfig,
                presenterFactory
            );
        }
    }
}
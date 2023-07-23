using NonebNi.Core.Agents;
using NonebNi.Core.Level;
using NonebNi.Terrain;
using NonebNi.Ui.Cameras;
using NonebNi.Ui.Statistics.Unit;

namespace NonebNi.Main
{
    public interface ILevelUi
    {
        void Run();
    }

    public class LevelUi : ILevelUi
    {
        private readonly CameraControl _cameraControl;
        private readonly Hud _hud;
        private readonly UnitDetailStat _stat;
        private readonly Terrain _terrain;

        public LevelUi(CameraControl cameraControl,
            Hud hud,
            Terrain terrain,
            UnitDetailStat stat,
            ICameraControllerView cameraControllerView,
            LevelData levelData,
            IPlayerAgent playerAgent,
            ITerrainMeshCreator meshCreator)
        {
            _cameraControl = cameraControl;
            _hud = hud;
            _terrain = terrain;
            _stat = stat;

            _cameraControl.Init(cameraControllerView);
            _hud.Init(levelData, playerAgent);
            _terrain.Init(meshCreator);
            _stat.Init();
        }

        public void Run()
        {
            _cameraControl.Run();
        }
    }
}
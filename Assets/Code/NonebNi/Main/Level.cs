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
        private readonly Grid _grid;
        private readonly Hud _hud;
        private readonly UnitDetailStat _stat;

        public LevelUi(CameraControl cameraControl,
            Hud hud,
            Grid grid,
            UnitDetailStat stat,
            ICameraControllerView cameraControllerView,
            LevelData levelData,
            IPlayerAgent playerAgent,
            ITerrainMeshCreator meshCreator)
        {
            _cameraControl = cameraControl;
            _hud = hud;
            _grid = grid;
            _stat = stat;

            _cameraControl.Init(cameraControllerView);
            _hud.Init(levelData, playerAgent);
            _grid.Init(meshCreator);
            _stat.Init();
        }

        public void Run()
        {
            _cameraControl.Run();
        }
    }
}
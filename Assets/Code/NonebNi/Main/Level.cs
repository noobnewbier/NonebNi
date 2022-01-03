using NonebNi.Core.Di;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;
using NonebNi.Ui.Di;
using NonebNi.Ui.Huds;
using NonebNi.Ui.Statistics.Unit;
using UnityEngine;
using Grid = NonebNi.Ui.Grids.Grid;

namespace NonebNi.Main
{
    public class Level : MonoBehaviour
    {
        [SerializeField] private LevelDataSource levelDataSource = null!;

        [SerializeField] private Hud hud = null!;
        [SerializeField] private Grid grid = null!;
        [SerializeField] private CameraControl cameraControl = null!;

        [SerializeField] private UnitDetailStat unitDetailStat = null!;

        private ILevelFlowController _levelFlowController = null!;

        private void Awake()
        {
            var levelModule = new LevelModule(levelDataSource.GetData());
            var commandEvaluationModule = new CommandEvaluationModule();
            var coordinateAndPositionServiceModule = new CoordinateAndPositionServiceModule(levelModule);

            var levelComponent = new LevelComponent(levelModule, commandEvaluationModule);

            _levelFlowController = levelComponent.GetLevelFlowController();

            hud.Init(new HudComponent(levelComponent));
            grid.Init(new GridComponent(coordinateAndPositionServiceModule, levelModule));
            cameraControl.Init(levelComponent, coordinateAndPositionServiceModule);

            unitDetailStat.Init();
        }

        private void Update()
        {
            _levelFlowController.UpdateState();
        }
    }
}
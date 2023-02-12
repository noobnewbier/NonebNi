using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;
using NonebNi.Main.Di;
using NonebNi.Ui.Statistics.Unit;
using UnityEngine;

namespace NonebNi.Main
{
    public class LevelRunner : MonoBehaviour
    {
        [SerializeField] private LevelDataSource levelDataSource = null!;

        [SerializeField] private Hud hud = null!;
        [SerializeField] private Grid grid = null!;
        [SerializeField] private CameraControl cameraControl = null!;

        [SerializeField] private UnitDetailStat unitDetailStat = null!;

        private ILevelUi _levelUi = null!;

        public LevelData? LevelData { get; private set; }
        public ILevelFlowController? LevelFlowController { get; private set; }

        private void Awake()
        {
            LevelData = levelDataSource.GetData();
            var levelContainer = new LevelContainer(
                cameraControl.Config,
                cameraControl.TargetCamera,
                LevelData,
                hud,
                cameraControl,
                grid,
                unitDetailStat
            );
            LevelFlowController = levelContainer.Resolve<ILevelFlowController>().Value;
            _levelUi = levelContainer.Resolve<ILevelUi>().Value;

            LevelFlowController.Run();
            _levelUi.Run();
        }
    }
}
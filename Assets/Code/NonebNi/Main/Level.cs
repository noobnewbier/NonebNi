using System.Collections;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;
using NonebNi.Main.Di;
using NonebNi.Ui.Statistics.Unit;
using UnityEngine;

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

        public LevelComponent? LevelComponent { get; private set; }

        private void Awake()
        {
            var levelModule = new LevelModule(levelDataSource.GetData());
            var coordinateAndPositionServiceModule = new CoordinateAndPositionServiceModule(levelModule);

            LevelComponent = new LevelComponent(levelModule);

            _levelFlowController = LevelComponent.GetLevelFlowController();

            hud.Init(new HudComponent(LevelComponent));
            grid.Init(new GridComponent(coordinateAndPositionServiceModule, levelModule));
            cameraControl.Init(LevelComponent, coordinateAndPositionServiceModule);

            unitDetailStat.Init();

            StartCoroutine(UpdateLevelFlowControllerRoutine());
        }

        private IEnumerator UpdateLevelFlowControllerRoutine()
        {
            while (true)
            {
                var routine = StartCoroutine(_levelFlowController.UpdateState());
                yield return routine;
            }
        }
    }
}
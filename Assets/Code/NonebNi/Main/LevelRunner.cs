using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;
using NonebNi.Main.Di;
using NonebNi.Terrain;
using NonebNi.Ui.Statistics.Unit;
using UnityEngine;

namespace NonebNi.Main
{
    public class LevelRunner : MonoBehaviour
    {
        [Header("UI")] [SerializeField] private Hud hud = null!;

        [SerializeField] private Terrain terrain = null!;
        [SerializeField] private UnitDetailStat unitDetailStat = null!;

        [Header("Level Data")] [SerializeField]
        private LevelDataSource levelDataSource = null!;

        [Header("Terrain")] [SerializeField] private TerrainConfigSource terrainConfig = null!;

        [Header("Camera")] [SerializeField] private CameraControl cameraControl = null!;


        private ILevelUi _levelUi = null!;
        public LevelData? LevelData { get; private set; }
        public ILevelFlowController? LevelFlowController { get; private set; }
        public TerrainConfigData? TerrainConfig { get; private set; }

        private void Awake()
        {
            LevelData = levelDataSource.GetData();
            TerrainConfig = terrainConfig.CreateData();

            var levelContainer = new LevelContainer(
                cameraControl.Config,
                cameraControl.TargetCamera,
                LevelData,
                hud,
                cameraControl,
                terrain,
                unitDetailStat,
                TerrainConfig,
                new TerrainMeshData()
            );
            LevelFlowController = levelContainer.Resolve<ILevelFlowController>().Value;
            _levelUi = levelContainer.Resolve<ILevelUi>().Value;

            LevelFlowController.Run();
            _levelUi.Run();
        }
    }
}
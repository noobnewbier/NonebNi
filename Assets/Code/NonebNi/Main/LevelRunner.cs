using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;
using NonebNi.Main.Di;
using NonebNi.Terrain;
using NonebNi.Ui.Grids;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using UnityEngine;

namespace NonebNi.Main
{
    public class LevelRunner : MonoBehaviour
    {
        [Header("UI"), SerializeField] private Hud hud = null!;

        [SerializeField] private Terrain terrain = null!;
        [SerializeField] private PlayerTurnMenu playerTurnMenu = null!;
        [SerializeField] private HexHighlightConfig hexHighlightConfig = new();


        [Header("Level Data"), SerializeField] private LevelDataSource levelDataSource = null!;

        [Header("Terrain"), SerializeField] private TerrainConfigSource terrainConfig = null!;

        [Header("Camera"), SerializeField] private CameraRunner cameraControl = null!;


        public ILevelUi LevelUi { get; private set; } = null!;
        public LevelData? LevelData { get; private set; }
        public ILevelFlowController? LevelFlowController { get; private set; }
        public TerrainConfigData? TerrainConfig { get; private set; }

        private void Awake()
        {
            LevelData = levelDataSource.GetData();
            TerrainConfig = terrainConfig.CreateData();

            //todo: this is all jank atm, we need to refactor this after we confirm ui works.
            //TODO: either level container create level flow control which needs level UI
            var levelContainer = new LevelContainer(
                cameraControl.Config,
                cameraControl,
                cameraControl.Composer,
                LevelData,
                hud,
                cameraControl.CinemachineCamera,
                terrain,
                TerrainConfig,
                new TerrainMeshData(),
                playerTurnMenu,
                cameraControl.Camera,
                hexHighlightConfig
            );
            LevelFlowController = levelContainer.Resolve<ILevelFlowController>().Value;
            LevelUi = levelContainer.Resolve<ILevelUi>().Value;

            var levelEventsReader = LevelFlowController.Run();
            LevelUi.Run(levelEventsReader);
        }
    }
}
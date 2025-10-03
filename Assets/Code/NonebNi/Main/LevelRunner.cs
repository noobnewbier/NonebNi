using System.Linq;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Actions;
using NonebNi.Core.Agents;
using NonebNi.Core.Factions;
using NonebNi.Core.FlowControl;
using NonebNi.Core.GameContexts;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Main.Di.Core;
using NonebNi.Main.Di.Debug;
using NonebNi.Main.Di.UI;
using NonebNi.Terrain;
using NonebNi.Ui.Cameras;
using NonebNi.Ui.Debug;
using NonebNi.Ui.Grids;
using NonebNi.Ui.Tooltips;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using StrongInject;
using StrongInject.Modules;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NonebNi.Main
{
    /// <summary>
    /// Note: feels like we can resolve ui deps in a separate container, have it static ? or level runner can make a static
    /// accessible thing.
    /// </summary>
    [
        RegisterModule(typeof(CoreModule)),
        RegisterModule(typeof(UIModule)),
        RegisterModule(typeof(DebugModule)),
        RegisterModule(typeof(ValueTupleModule))
    ]
    public partial class LevelRunner : MonoBehaviour, IAsyncContainer<(ILevelFlowController, ILevelUi, DebugTools)>
    {
        [Header("UI")]
        [Instance, SerializeField] private Hud hud = null!;
        [Instance, SerializeField] private Terrain terrain = null!;
        [Instance(Options.AsImplementedInterfaces), SerializeField] private PlayerTurnMenu playerTurnMenu = null!;
        [Instance, SerializeField] private HexHighlightConfig hexHighlightConfig = new ();
        [Instance(Options.AsImplementedInterfaces), SerializeField] private TooltipCanvas tooltipCanvas = null!;
        [Instance, SerializeField] private CanvasRoot canvasRoot = null!;
        [Instance, SerializeField] private InputActionAsset inputActionAsset = null!;
        [Instance, SerializeField] private InfluenceHighlight influenceHighlight = null!;
        [Instance, SerializeField] private CameraRunner cameraControl = null!;

        [Header("Level Data")]
        [Instance, SerializeField] private LevelDataSource levelDataSource = null!;
        [Instance, SerializeField] private TerrainConfigSource terrainConfig = null!;

        // Non serialized fields.
        [Instance] private IAgent[] _agents = null!;
        [Instance] private LevelData _levelData = null!; //todo: do I need to this?
        [Instance] private KeyedInject<DiKeys.PlayerAgent, IWaitForExternalInputAgent> _playerAgent = null!;
        [Instance] private TerrainConfigData _terrainConfig = null!;
        [Instance] private TerrainMeshData _terrainMeshData = null!;

        //Note: camera stuffs - can/should I combine them?
        [Instance] private CinemachineCamera CinemachineCamera => cameraControl.CinemachineCamera;
        [Instance] private CinemachinePositionComposer Composer => cameraControl.Composer;
        [Instance] private CameraControlSetting Config => cameraControl.Config;
        [Instance] private Camera Camera => cameraControl.Camera;
        [Instance] private NonebAction[] Actions => ActionDatas.Actions;
        [Instance] private Faction[] Factions => _levelData.Factions;
        [Instance(Options.AsImplementedInterfaces)] private IMap Map => _levelData.Map;

        public DebugTools? DebugTools { get; private set; }

        private void Awake()
        {
            Do().Forget();

            return;

            async UniTaskVoid Do()
            {
                _terrainMeshData = new TerrainMeshData();
                _levelData = levelDataSource.GetData();
                _terrainConfig = terrainConfig.CreateData();
                _agents = _levelData.Factions.Select(f =>
                    {
                        IAgent agent = f.IsPlayerControlled ?
                            new WaitForExternalInputAgent(f) :
                            new DummyAgent(f);

                        return agent;
                    }
                ).ToArray();
                _playerAgent = new (_agents.OfType<IWaitForExternalInputAgent>().First(a => a.Faction.IsPlayerControlled));

                /*
                 * Note:
                 * this is all jank atm, we need to refactor this after we confirm ui works.
                 * either level container create level flow control which needs level UI
                 * --
                 * Can confirm, I have no idea what I was talking about, probs should delete this comment?
                 */
                IAsyncContainer<(ILevelFlowController, ILevelUi, DebugTools)> container = this;
                var (levelFlowController, levelUi, debugTools) = (await container.ResolveAsync()).Value;
                DebugTools = debugTools;

                levelFlowController.Run().Forget();
                levelUi.Run();
            }
        }
    }
}
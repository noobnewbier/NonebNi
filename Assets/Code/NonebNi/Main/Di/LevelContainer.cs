using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Agents;
using NonebNi.Core.Factions;
using NonebNi.Core.FlowControl;
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

namespace NonebNi.Main.Di
{
    /// <summary>
    /// Note:
    /// i got a feeling that I can just make it an IContainer of both core, ui and debug, this limits the amount of "fuck
    /// Editor needs Core's module for something"
    /// </summary>
    [RegisterModule(typeof(UIModule))]
    [RegisterModule(typeof(DebugModule))]
    [RegisterModule(typeof(CoreModule))]
    [RegisterModule(typeof(ValueTupleModule))]
    public partial class LevelContainer : IAsyncContainer<(ILevelFlowController, ILevelUi, DebugTools)>
    {
        [Instance] private readonly IAgent[] _agents;
        [Instance] private readonly CinemachineCamera _camera;
        [Instance] private readonly CameraRunner _cameraControl;
        [Instance] private readonly CanvasRoot _canvasRoot;
        [Instance] private readonly CinemachinePositionComposer _composer;
        [Instance] private readonly CameraControlSetting _config;
        [Instance] private readonly HexHighlightConfig _hexHighlightConfig;

        [Instance] private readonly Hud _hud;

        //todo: put this into where they belong
        [Instance] private readonly InfluenceHighlight _influenceHighlight;
        [Instance] private readonly InputActionAsset _inputAsset;
        [Instance] private readonly Camera _levelCamera; //todo: feels like it shouldn't be here
        [Instance] private readonly LevelData _levelData;
        [Instance] private readonly IPlayerTurnMenu _playerTurnMenu; //todo: need?
        [Instance] private readonly Terrain _terrain;
        [Instance] private readonly TerrainConfigData _terrainConfig;
        [Instance] private readonly TerrainMeshData _terrainMeshData;
        [Instance] private readonly ITooltipCanvas _tooltipCanvas;

        public LevelContainer(
            CameraControlSetting config,
            CameraRunner cameraControl,
            CinemachinePositionComposer composer,
            LevelData levelData,
            Hud hud,
            CinemachineCamera camera,
            Terrain terrain,
            TerrainConfigData terrainConfig,
            TerrainMeshData terrainMeshData,
            IPlayerTurnMenu playerTurnMenu,
            Camera levelCamera,
            HexHighlightConfig hexHighlightConfig,
            ITooltipCanvas tooltipCanvas,
            CanvasRoot canvasRoot,
            InputActionAsset inputAsset,
            InfluenceHighlight influenceHighlight)
        {
            _config = config;
            _levelData = levelData;
            _hud = hud;
            _cameraControl = cameraControl;
            _terrain = terrain;
            _terrainConfig = terrainConfig;
            _terrainMeshData = terrainMeshData;
            _playerTurnMenu = playerTurnMenu;
            _levelCamera = levelCamera;
            _hexHighlightConfig = hexHighlightConfig;
            _tooltipCanvas = tooltipCanvas;
            _canvasRoot = canvasRoot;
            _inputAsset = inputAsset;
            _influenceHighlight = influenceHighlight;
            _camera = camera;
            _composer = composer;
            _agents = _levelData.Factions.Select(
                f =>
                {
                    IAgent agent = f.IsPlayerControlled ?
                        new PlayerAgent(f) :
                        new DummyAgent(f);

                    return agent;
                }
            ).ToArray();
        }

        [Instance] private NonebAction[] Actions => ActionDatas.Actions;

        [Instance] private Faction[] Factions => _levelData.Factions;
        [Instance] private IMap Map => _levelData.Map;
        [Instance] private IReadOnlyMap ReadOnlyMap => _levelData.Map;
    }
}
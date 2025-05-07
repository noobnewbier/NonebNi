using System.Linq;
using NonebNi.Core.Agents;
using NonebNi.Core.Factions;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Terrain;
using NonebNi.Ui.Cameras;
using NonebNi.Ui.Grids;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using StrongInject;
using StrongInject.Modules;
using Unity.Cinemachine;
using UnityEngine;

namespace NonebNi.Main.Di
{
    //todo: i got a feeling that I can just make it an IContainer of both core, ui and debug, this limits the amount of "fuck Editor needs Core's module for something" 
    [RegisterModule(typeof(AgentsModule))]
    [RegisterModule(typeof(CameraControllerModule))]
    [RegisterModule(typeof(CoordinateAndPositionServiceModule))]
    [RegisterModule(typeof(LevelFlowControlModule))]
    [RegisterModule(typeof(UIModule))]
    [Register(typeof(LevelUi), typeof(ILevelUi))]
    [Register(typeof(TerrainMeshCreator), typeof(ITerrainMeshCreator))]
    [RegisterModule(typeof(ValueTupleModule))]
    public partial class LevelContainer : IContainer<(ILevelFlowController, ILevelUi)>
    {
        [Instance] private readonly IAgent[] _agents;
        [Instance] private readonly CinemachineCamera _camera;
        [Instance] private readonly CameraRunner _cameraControl;
        [Instance] private readonly CinemachinePositionComposer _composer;
        [Instance] private readonly CameraControlSetting _config;
        [Instance] private readonly HexHighlightConfig _hexHighlightConfig;
        [Instance] private readonly Hud _hud;
        [Instance] private readonly Camera _levelCamera; //todo: feels like it shouldn't be here
        [Instance] private readonly LevelData _levelData;
        [Instance] private readonly IPlayerTurnMenu _playerTurnMenu;
        [Instance] private readonly Terrain _terrain;
        [Instance] private readonly TerrainConfigData _terrainConfig;
        [Instance] private readonly TerrainMeshData _terrainMeshData;

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
            HexHighlightConfig hexHighlightConfig)
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

        [Instance] private Faction[] Factions => _levelData.Factions;
        [Instance] private IMap Map => _levelData.Map;
        [Instance] private IReadOnlyMap ReadOnlyMap => _levelData.Map;
    }
}
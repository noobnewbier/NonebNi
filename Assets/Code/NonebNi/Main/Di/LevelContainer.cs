using System.Linq;
using NonebNi.Core.Agents;
using NonebNi.Core.Factions;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Terrain;
using NonebNi.Ui.Cameras;
using NonebNi.Ui.Statistics.Unit;
using StrongInject;
using UnityEngine;

namespace NonebNi.Main.Di
{
    [RegisterModule(typeof(AgentsModule))]
    [RegisterModule(typeof(CameraControlViewModule))]
    [RegisterModule(typeof(CoordinateAndPositionServiceModule))]
    [RegisterModule(typeof(LevelFlowControlModule))]
    [Register(typeof(LevelUi), typeof(ILevelUi))]
    [Register(typeof(TerrainMeshCreator), typeof(ITerrainMeshCreator))]
    public partial class LevelContainer : IContainer<ILevelUi>, IContainer<ILevelFlowController>
    {
        [Instance] private readonly IAgent[] _agents;
        [Instance] private readonly CameraControl _cameraControl;
        [Instance] private readonly CameraConfig _config;
        [Instance] private readonly Hud _hud;
        [Instance] private readonly LevelData _levelData;
        [Instance] private readonly Camera _targetCamera;
        [Instance] private readonly Terrain _terrain;
        [Instance] private readonly TerrainConfigData _terrainConfig;
        [Instance] private readonly TerrainMeshData _terrainMeshData;
        [Instance] private readonly UnitDetailStat _unitDetailStat;

        public LevelContainer(
            CameraConfig config,
            Camera targetCamera,
            LevelData levelData,
            Hud hud,
            CameraControl cameraControl,
            Terrain terrain,
            UnitDetailStat unitDetailStat,
            TerrainConfigData terrainConfig,
            TerrainMeshData terrainMeshData)
        {
            _config = config;
            _targetCamera = targetCamera;
            _levelData = levelData;
            _hud = hud;
            _cameraControl = cameraControl;
            _terrain = terrain;
            _unitDetailStat = unitDetailStat;
            _terrainConfig = terrainConfig;
            _terrainMeshData = terrainMeshData;
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
using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
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
    public partial class LevelContainer : IContainer<ILevelUi>, IContainer<ILevelFlowController>
    {
        [Instance] private readonly CameraControl _cameraControl;
        [Instance] private readonly CameraConfig _config;
        [Instance] private readonly Grid _grid;
        [Instance] private readonly Hud _hud;
        [Instance] private readonly LevelData _levelData;
        [Instance] private readonly Camera _targetCamera;
        [Instance] private readonly UnitDetailStat _unitDetailStat;

        public LevelContainer(CameraConfig config,
            Camera targetCamera,
            LevelData levelData,
            Hud hud,
            CameraControl cameraControl,
            Grid grid,
            UnitDetailStat unitDetailStat)
        {
            _config = config;
            _targetCamera = targetCamera;
            _levelData = levelData;
            _hud = hud;
            _cameraControl = cameraControl;
            _grid = grid;
            _unitDetailStat = unitDetailStat;
        }

        [Instance] private IMap Map => _levelData.Map;
        [Instance] private IReadOnlyMap ReadOnlyMap => _levelData.Map;
        [Instance] private WorldConfigData WorldConfig => _levelData.WorldConfig;
    }
}
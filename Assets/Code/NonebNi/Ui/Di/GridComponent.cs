using System;
using NonebNi.Core.Level;

namespace NonebNi.Ui.Di
{
    public interface IGridComponent
    {
        CoordinateAndPositionService CoordinateAndPositionService { get; }
        LevelData GetLevelData();
    }

    public class GridComponent : IGridComponent
    {
        private readonly GameModule _gameModule;
        private readonly Lazy<CoordinateAndPositionService> _lazyCoordinateAndPositionService;

        public GridComponent(GameModule gameModule)
        {
            _gameModule = gameModule;
            _lazyCoordinateAndPositionService =
                new Lazy<CoordinateAndPositionService>(() => _gameModule.GetCoordinateAndPositionService());
        }

        public LevelData GetLevelData() => _gameModule.LevelData;
        public CoordinateAndPositionService CoordinateAndPositionService => _lazyCoordinateAndPositionService.Value;
    }
}
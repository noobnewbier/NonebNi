using System;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Di;
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
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly CoordinateAndPositionServiceModule _coordinateAndPositionServiceModule;

        private readonly Lazy<CoordinateAndPositionService> _lazyCoordinateAndPositionService;
        private readonly LevelModule _levelModule;

        public GridComponent(CoordinateAndPositionServiceModule coordinateAndPositionServiceModule, LevelModule levelModule)
        {
            _coordinateAndPositionServiceModule = coordinateAndPositionServiceModule;
            _levelModule = levelModule;
            _lazyCoordinateAndPositionService = new Lazy<CoordinateAndPositionService>(
                () => _coordinateAndPositionServiceModule.GetCoordinateAndPositionService(_levelModule.LevelData)
            );
        }

        public LevelData GetLevelData() => _levelModule.LevelData;
        public CoordinateAndPositionService CoordinateAndPositionService => _lazyCoordinateAndPositionService.Value;
    }
}
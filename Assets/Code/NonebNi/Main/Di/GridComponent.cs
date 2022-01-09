using System;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Level;

namespace NonebNi.Main.Di
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
                () => _coordinateAndPositionServiceModule.GetCoordinateAndPositionService()
            );
        }

        public LevelData GetLevelData() => _levelModule.LevelData;
        public CoordinateAndPositionService CoordinateAndPositionService => _lazyCoordinateAndPositionService.Value;
    }
}
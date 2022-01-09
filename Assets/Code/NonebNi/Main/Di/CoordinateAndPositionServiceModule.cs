using System;
using NonebNi.Core.Coordinates;

namespace NonebNi.Main.Di
{
    public class CoordinateAndPositionServiceModule
    {
        private readonly Lazy<CoordinateAndPositionService> _lazy;
        private readonly LevelModule _levelModule;

        public CoordinateAndPositionServiceModule(LevelModule levelModule)
        {
            _levelModule = levelModule;
            _lazy = new Lazy<CoordinateAndPositionService>(
                () => new CoordinateAndPositionService(_levelModule.LevelData.WorldConfig)
            );
        }

        public CoordinateAndPositionService GetCoordinateAndPositionService() =>
            _lazy.Value;
    }
}
using System;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Level;

namespace NonebNi.Main.Di
{
    public class CoordinateAndPositionServiceModule
    {
        private readonly Lazy<CoordinateAndPositionService> _lazy;

        public CoordinateAndPositionServiceModule(WorldConfigData worldConfig)
        {
            _lazy = new Lazy<CoordinateAndPositionService>(
                () => new CoordinateAndPositionService(worldConfig)
            );
        }

        public CoordinateAndPositionService GetCoordinateAndPositionService() => _lazy.Value;
    }
}
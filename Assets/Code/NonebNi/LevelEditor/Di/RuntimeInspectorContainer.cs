using NonebNi.Core.Maps;
using NonebNi.LevelEditor.Inspectors;
using NonebNi.Terrain;
using StrongInject;

namespace NonebNi.LevelEditor.Di
{
    [RegisterModule(typeof(TileInspectorModule))]
    [RegisterModule(typeof(GridModule))]
    [Register(typeof(NonebEditorModel))]
    [Register(typeof(CoordinateAndPositionService), typeof(ICoordinateAndPositionService))]
    [Register(typeof(LevelInspector))]
    public partial class RuntimeInspectorContainer : IContainer<LevelInspector>
    {
        [Instance] private readonly IReadOnlyMap _map;
        [Instance] private readonly TerrainConfigData _terrainConfig;

        public RuntimeInspectorContainer(TerrainConfigData terrainConfig, IReadOnlyMap map)
        {
            _terrainConfig = terrainConfig;
            _map = map;
        }
    }
}
using NonebNi.Core.Maps;
using NonebNi.LevelEditor.Inspectors;
using NonebNi.Terrain;
using StrongInject;

namespace NonebNi.LevelEditor.Di
{
    /// <summary>
    /// It would be nice if the main assembly(like somehow bake it in DebugTools) can give us this as well.
    /// That said, I can't come up with a way to do this without using #if UNITY_EDITOR.
    /// ---
    /// Note
    /// - For now we only put the dependencies in debug tools, the code copypasta is not that bad yet
    /// - when the times comes put it in using ifdef UNITY_EDITOR, it's gross but doable. Or maybe by then I will have another
    /// plan.
    /// </summary>
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
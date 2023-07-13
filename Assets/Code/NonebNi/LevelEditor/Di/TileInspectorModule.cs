using NonebNi.LevelEditor.Level.Maps;
using NonebNi.LevelEditor.Level.Tiles;
using NonebNi.Terrain;
using StrongInject;
using UnityUtils.Factories;

namespace NonebNi.LevelEditor.Di
{
    public class TileInspectorModule
    {
        [Factory]
        public static TileInspectorView ProvideTileInspectorView(IEditorMap map,
            NonebEditorModel nonebEditorModel,
            ICoordinateAndPositionService coordinateAndPositionService,
            TerrainConfigData terrainConfig)
        {
            var presenterFactory = Factory.Create<TileInspectorView, TileInspectorPresenter>(
                view => new TileInspectorPresenter(view, coordinateAndPositionService, nonebEditorModel)
            );

            return new TileInspectorView(presenterFactory, terrainConfig, map);
        }
    }
}
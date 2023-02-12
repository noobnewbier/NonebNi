using NonebNi.Core.Coordinates;
using NonebNi.Core.Level;
using NonebNi.LevelEditor.Level.Data;
using NonebNi.LevelEditor.Level.Inspector;
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
            WorldConfigData worldConfig)
        {
            var presenterFactory = Factory.Create<TileInspectorView, TileInspectorPresenter>(
                view => new TileInspectorPresenter(view, coordinateAndPositionService, nonebEditorModel)
            );

            return new TileInspectorView(presenterFactory, worldConfig, map);
        }
    }
}
using NonebNi.Core.Maps;
using NonebNi.LevelEditor.Level.Maps;
using NonebNi.Terrain;
using StrongInject;
using UnityUtils.Factories;

namespace NonebNi.LevelEditor.Di
{
    public class GridModule
    {
        [Factory]
        public static GridView ProvideGridView(
            NonebEditorModel nonebEditorModel,
            ICoordinateAndPositionService coordinateAndPositionService,
            IReadOnlyMap map)
        {
            var presenterFactory = Factory.Create<GridView, GridPresenter>(
                view => new GridPresenter(view, nonebEditorModel, map)
            );

            return new GridView(presenterFactory, coordinateAndPositionService);
        }
    }
}
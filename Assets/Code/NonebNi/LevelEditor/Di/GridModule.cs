using NonebNi.Core.Coordinates;
using NonebNi.Core.Level;
using NonebNi.LevelEditor.Level;
using NonebNi.LevelEditor.Level.Maps;
using StrongInject;
using UnityUtils.Factories;

namespace NonebNi.LevelEditor.Di
{
    public class GridModule
    {
        [Factory]
        public static GridView ProvideGridView(LevelEditorModel levelEditorModel,
            NonebEditorModel nonebEditorModel,
            ICoordinateAndPositionService coordinateAndPositionService,
            WorldConfigData worldConfig)
        {
            var presenterFactory = Factory.Create<GridView, GridPresenter>(
                view => new GridPresenter(view, levelEditorModel, nonebEditorModel)
            );

            return new GridView(presenterFactory, coordinateAndPositionService, worldConfig);
        }
    }
}
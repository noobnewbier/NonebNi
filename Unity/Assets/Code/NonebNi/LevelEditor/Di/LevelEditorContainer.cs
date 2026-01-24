using System;
using NonebNi.LevelEditor.Level;
using NonebNi.LevelEditor.Level.Entities;
using NonebNi.LevelEditor.Level.Error;
using NonebNi.LevelEditor.Level.Maps;
using NonebNi.LevelEditor.Level.Settings;
using NonebNi.Terrain;
using StrongInject;
using UnityEngine.SceneManagement;

namespace NonebNi.LevelEditor.Di
{
    [RegisterModule(typeof(TileInspectorModule))]
    [RegisterModule(typeof(GridModule))]
    [RegisterModule(typeof(ErrorOverviewModule))]
    [Register(typeof(CoordinateAndPositionService), typeof(ICoordinateAndPositionService))]
    [Register(typeof(LevelEditorModel))]
    [Register(typeof(EditorEntitySyncService))]
    [Register(typeof(EntitiesPlacer))]
    [Register(typeof(EditorEntityPositioningService))]
    [Register(typeof(ErrorChecker))]
    [Register(typeof(LevelSavingService))]
    [Register(typeof(LevelDataSyncer))]
    [Register(typeof(Level.LevelEditor))]
    public partial class LevelEditorContainer : IContainer<Level.LevelEditor>
    {
        [Instance] private readonly EditorLevelData _editorLevelData;
        [Instance] private readonly EditorLevelDataSource _levelDataSource;
        [Instance] private readonly NonebEditorModel _nonebEditorModel;
        [Instance] private readonly Scene _scene;

        public LevelEditorContainer(
            EditorLevelDataSource levelDataSource,
            Scene scene,
            NonebEditorModel nonebEditorModel,
            EditorLevelData editorLevelData)
        {
            if (!levelDataSource.IsValid)
                throw new ArgumentException(
                    $"{nameof(levelDataSource)} must be valid(${nameof(levelDataSource.IsValid)} should be true, otherwise no backing data can be created)"
                );

            _levelDataSource = levelDataSource;
            _scene = scene;
            _nonebEditorModel = nonebEditorModel;
            _editorLevelData = editorLevelData;
        }

        [Instance(Options.AsImplementedInterfacesAndBaseClasses)]
        private EditorMap EditorMap => _editorLevelData.Map;

        [Instance] private TerrainConfigData TerrainConfig => _editorLevelData.TerrainConfig;
    }
}
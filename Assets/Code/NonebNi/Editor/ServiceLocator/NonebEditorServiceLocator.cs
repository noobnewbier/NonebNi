using System;
using NonebNi.Core.Maps;
using NonebNi.Editor.Level;

namespace NonebNi.Editor.ServiceLocator
{
    /// <summary>
    /// To the future me:
    /// ---
    /// At the moment the only editor we have is the level editor,
    /// The overhead of creating a manual Di system is just not worth it for now.
    /// Knowing myself, the future me is probably looking at this and be like "wth is wrong with my younger-self", the structure is horrific etc...
    /// But before refactoring, please think twice, as your younger-self has already gone through that process and be like "nah it's not worth it"
    /// When the time comes where we have another important editor, where states needs to be shared between various editor,
    /// we should probably create a manual Di system(mimicking Dagger2 but doing everything manually), but for now this should work.#
    /// ---
    /// </summary>
    public class NonebEditorServiceLocator
    {
        private static readonly Lazy<NonebEditorServiceLocator> LazyInstance = new Lazy<NonebEditorServiceLocator>();

        private readonly Lazy<LevelEditorDataModel> _lazyEditorDataModel = new Lazy<LevelEditorDataModel>();
        private readonly Lazy<MapGenerationService> _lazyMapGenerationService = new Lazy<MapGenerationService>();
        public static NonebEditorServiceLocator Instance => LazyInstance.Value;
        public LevelEditorDataModel LevelEditorDataModel => _lazyEditorDataModel.Value;
        public MapGenerationService MapGenerationService => _lazyMapGenerationService.Value;
    }
}
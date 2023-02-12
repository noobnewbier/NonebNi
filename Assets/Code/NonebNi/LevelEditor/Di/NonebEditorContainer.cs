using StrongInject;

namespace NonebNi.LevelEditor.Di
{
    [Register(typeof(NonebEditor))]
    [Register(typeof(NonebEditorModel))]
    [RegisterModule(typeof(NonebEditorToolbarModule))]
    public partial class NonebEditorContainer : IContainer<NonebEditor> { }
}
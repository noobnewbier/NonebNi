using NonebNi.LevelEditor.Inspectors;
using StrongInject;

namespace NonebNi.LevelEditor.Di
{
    [Register(typeof(NonebInspector))]
    public partial class NonebInspectorContainer : IContainer<NonebInspector> { }
}
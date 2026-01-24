using NonebNi.LevelEditor.Toolbar;
using StrongInject;
using UnityUtils.Factories;

namespace NonebNi.LevelEditor.Di
{
    public class NonebEditorToolbarModule
    {
        [Factory]
        public static NonebEditorToolbarView NonebEditorToolbarView(NonebEditorModel editorModel)
        {
            var presenterFactory = Factory.Create<NonebEditorToolbarView, NonebEditorToolbarPresenter>(
                view => new NonebEditorToolbarPresenter(view, editorModel)
            );

            return new NonebEditorToolbarView(presenterFactory);
        }
    }
}
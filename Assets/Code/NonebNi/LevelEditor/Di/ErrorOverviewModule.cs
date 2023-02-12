using NonebNi.LevelEditor.Level.Error;
using StrongInject;
using UnityUtils.Factories;

namespace NonebNi.LevelEditor.Di
{
    public class ErrorOverviewModule
    {
        [Factory]
        public static ErrorOverviewView ProvideErrorOverviewView(ErrorChecker errorChecker, NonebEditorModel editorModel)
        {
            var presenterFactory = Factory.Create<ErrorOverviewView, ErrorOverviewPresenter>(
                view => new ErrorOverviewPresenter(view, errorChecker, editorModel)
            );

            return new ErrorOverviewView(presenterFactory);
        }
    }
}
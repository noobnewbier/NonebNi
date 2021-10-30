using System.Collections.Generic;

namespace NonebNi.Editors.Level.Error
{
    public class ErrorOverviewPresenter
    {
        private readonly ErrorChecker _errorChecker;
        private readonly ErrorOverviewView _view;

        public bool IsDrawing { get; } = true;
        public IEnumerable<ErrorEntry> ErrorEntries => _errorChecker.CheckForErrors();

        public ErrorOverviewPresenter(ErrorOverviewView view, ErrorChecker errorChecker)
        {
            _view = view;
            _errorChecker = errorChecker;
        }
    }
}
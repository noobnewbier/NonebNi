using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Editors.Level.Error
{
    public class ErrorOverviewPresenter
    {
        private readonly ErrorChecker _errorChecker;
        private readonly NonebEditorModel _nonebEditorModel;
        private readonly ErrorOverviewView _view;
        public bool IsDrawing => _nonebEditorModel.IsHelperWindowsVisible;
        public IEnumerable<ErrorEntry> ErrorEntries => _errorChecker.CheckForErrors();

        public ErrorOverviewPresenter(ErrorOverviewView view, ErrorChecker errorChecker, NonebEditorModel nonebEditorModel)
        {
            _view = view;
            _errorChecker = errorChecker;
            _nonebEditorModel = nonebEditorModel;
        }

        public void OnClickErrorNavigationButton(ErrorEntry entry)
        {
            Selection.SetActiveObjectWithContext(entry.ErrorSource, entry.ErrorSource);
            EditorGUIUtility.PingObject(entry.ErrorSource);

            var sceneView = SceneView.lastActiveSceneView;

            var errorPosition = entry.ErrorSource.transform.position;
            var dir = (errorPosition - sceneView.camera.transform.position).normalized;
            var targetRotation = Quaternion.LookRotation(dir);

            const int zoom = 7; //magic number
            sceneView.LookAt(errorPosition, targetRotation, zoom);
        }
    }
}
using System.Collections.Generic;
using Code.NonebNi.EditorComponent.Entities;
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

        public void OnClickErrorNavigationButton(Entity errorEntity)
        {
            Selection.SetActiveObjectWithContext(errorEntity, errorEntity);
            EditorGUIUtility.PingObject(errorEntity);

            var sceneView = SceneView.lastActiveSceneView;

            var errorPosition = errorEntity.transform.position;
            var dir = (errorPosition - sceneView.camera.transform.position).normalized;
            var targetRotation = Quaternion.LookRotation(dir);

            const int zoom = 7; //magic number
            sceneView.LookAt(errorPosition, targetRotation, zoom);
        }
    }
}
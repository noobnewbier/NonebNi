﻿using System.Collections.Generic;
using NonebNi.EditorComponent.Entities;
using UnityEditor;
using UnityEngine;

namespace NonebNi.LevelEditor.Level.Error
{
    public class ErrorOverviewPresenter
    {
        private readonly ErrorChecker _errorChecker;
        private readonly NonebEditorModel _nonebEditorModel;
        private readonly ErrorOverviewView _view;

        public ErrorOverviewPresenter(ErrorOverviewView view, ErrorChecker errorChecker, NonebEditorModel nonebEditorModel)
        {
            _view = view;
            _errorChecker = errorChecker;
            _nonebEditorModel = nonebEditorModel;
        }

        public bool IsDrawing => _nonebEditorModel.IsHelperWindowsVisible;
        public IEnumerable<ErrorEntry> ErrorEntries => _errorChecker.CheckForErrors();

        public void OnClickErrorNavigationButton(EditorEntity errorEditorEntity)
        {
            Selection.SetActiveObjectWithContext(errorEditorEntity, errorEditorEntity);
            EditorGUIUtility.PingObject(errorEditorEntity);

            var sceneView = SceneView.lastActiveSceneView;

            var errorPosition = errorEditorEntity.transform.position;
            var dir = (errorPosition - sceneView.camera.transform.position).normalized;
            var targetRotation = Quaternion.LookRotation(dir);

            const int zoom = 7; //magic number
            sceneView.LookAt(errorPosition, targetRotation, zoom);
        }
    }
}
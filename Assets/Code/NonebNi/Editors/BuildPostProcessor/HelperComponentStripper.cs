using System.Linq;
using NonebNi.Editors.Level.Entities;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NonebNi.Editors.BuildPostProcessor
{
    public class HelperComponentStripper : IProcessSceneWithReport
    {
        public int callbackOrder { get; }

        public void OnProcessScene(Scene scene, BuildReport report)
        {
            var roots = scene.GetRootGameObjects();

            var entities = roots.SelectMany(r => r.GetComponentsInChildren<Entity>());
            foreach (var entity in entities) Object.DestroyImmediate(entity);
        }
    }
}
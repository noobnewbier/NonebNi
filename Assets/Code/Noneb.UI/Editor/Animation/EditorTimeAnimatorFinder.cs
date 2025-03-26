using Unity.Logging;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace Noneb.UI.Editor.Animation
{
    public class EditorTimeAnimatorFinder
    {
        public static Animator? FindForInspector(SerializedProperty property, string animatorName, bool useRootObjectField)
        {
            Animator? animator;
            if (useRootObjectField)
            {
                if (string.IsNullOrEmpty(animatorName))
                    animator = property.serializedObject.FindPropertyOfTypeAtRoot<Animator>();
                else
                    animator = property.serializedObject.FindProperty(animatorName).objectReferenceValue as Animator;

                if (animator == null)
                {
                    //Last ditch effort - grab from the object itself(if it's a mono). Don't look into the child object as it's gonna be confusing af
                    var targetMono = property.serializedObject.targetObject as MonoBehaviour;
                    if (targetMono != null)
                    {
                        var gameObject = targetMono.gameObject;
                        animator = gameObject.GetComponent<Animator>();
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(animatorName))
                {
                    animator = null;
                    Log.Error(
                        "We cannot find animator automatically when you are not referencing the root type, you must define animator name before I decide to implement this."
                    );
                }
                else
                {
                    animator = NonebEditorUtils.FindPropertyObjectReferenceInSameDepth<Animator>(property, animatorName);
                }
            }

            return animator;
        }
    }
}
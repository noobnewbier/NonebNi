using Unity.Behavior.GraphFramework;
using Unity.Logging;
using UnityEditor;

namespace NonebNi.EditorScripting
{
    internal static class PrintSerializableGuid
    {
        [MenuItem("NonebNi/PrintSerializableGuid")]
        public static void Print()
        {
            /*
             * Extending Unity.Behaviour.Node requires us to tag it with an attribute with the serializable guid assigned
             * There's of course, no file template for it, so you can either step through the wizard like a layman or use this to print the thing and copy-paste like a layman.
             * Your choice my friend.
             */
            var guid = SerializableGUID.Generate().ToString();
            EditorGUIUtility.systemCopyBuffer = guid;
            Log.Info($"{guid} is now in your clipboard!");
        }
    }
}
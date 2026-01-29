using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.EditorTools
{
    /// <summary>
    /// Credit: https://github.com/Emerix/GradientToTexture/blob/master/Assets/Editor/GradientToTexture.cs
    /// http://answers.unity3d.com/questions/461958/generate-a-gradient-texture-from-editor-script.html
    /// http://answers.unity3d.com/questions/436295/how-to-have-a-gradient-editor-in-an-editor-script.html
    /// </summary>
    public class GradientTextureGenerator : EditorWindow
    {
        [SerializeField] private Gradient gradient = new ();
        [SerializeField, Range(0, 4096)] private int width = 128;

        private NonebGUIDrawer? _drawer;

        [MenuItem("NonebNi/Gradient Texture Generator")]
        private static void Init()
        {
            var window = (GradientTextureGenerator)GetWindow(typeof(GradientTextureGenerator));

            window.maxSize = new (300, 200);
        }

        private void OnGUI()
        {
            _drawer ??= new (this);
            
            _drawer.Update();

            _drawer.DrawProperty(nameof(gradient));
            _drawer.DrawProperty(nameof(width));

            _drawer.Apply();

            var tex = new Texture2D(width, 1);
            for (var i = 0; i < width; i++) tex.SetPixel(i, 0, gradient.Evaluate(i / (float)width));
            if (GUILayout.Button("Generate"))
            {
                var path = EditorUtility.SaveFilePanel("Save texture as PNG", "", "foo.png", "png");
                if (path.Length != 0) Generate(tex, path);
            }
        }

        private void Generate(Texture2D tex, string path)
        {
            var png = tex.EncodeToPNG()!;
            NonebEditorGUI.CreateFile(path, png);
        }
    }
}
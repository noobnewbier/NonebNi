using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace NonebNi.Ui.Debug
{
    //todo: need to implement debug menu: https://github.com/vape/Undebugger/blob/master/Runtime/Scripts/Services/UI/UIService.cs  at some point?
    public class InfluenceHighlight : MonoBehaviour
    {
        //Note: a tool for this would be nice - would prefer assigning this in editor.
        private static readonly int BaseMap = Shader.PropertyToID("Base_Map");
        private static readonly int Color = Shader.PropertyToID("_Color");
        [SerializeField] private DecalProjector decal = null!;


        private void Awake()
        {
            decal.material = new Material(decal.material);
        }

        public void Draw(Color color, Texture2D hexagonTex)
        {
            decal.material.SetColor(Color, color);
            decal.material.SetTexture(BaseMap, hexagonTex);
        }
    }
}
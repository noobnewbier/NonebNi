using NonebNi.Ui.Grids;
using UnityEngine;

namespace NonebNi.Develop
{
    public class HighlightTestScript : MonoBehaviour
    {
        [SerializeField] private HexHighlight highlight;
        [SerializeField, Range(1, 10)]  private float radius;


        private void Update()
        {
            highlight.Draw(radius);
        }
    }
}
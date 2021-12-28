using System.Linq;
using NonebNi.Core.Level;
using UnityEngine;

namespace NonebNi.Ui.Grids
{
    public class TileView : MonoBehaviour
    {
        [SerializeField] private LineRenderer lineRenderer = null!;

        public void Init(Vector3 position, WorldConfigData worldConfigData)
        {
            transform.position = position;

            var corners = worldConfigData.TileCornersOffset.Select(
                /*
                 * Need to rotate the corners to Z-up oriented, as the line renderer's line is rendered in a z-facing faction.
                 * Unity doesn't allow us to customize this to face the direction we want.
                 */
                c => Quaternion.LookRotation(Vector3.up, Vector3.back) * c
            );

            lineRenderer.positionCount = 6; //set position alone won't do it
            lineRenderer.SetPositions(corners.ToArray());
        }
    }
}
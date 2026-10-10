using System;
using UnityEngine;

namespace Code
{
    [ExecuteAlways]
    public class OrthoSizeTest : MonoBehaviour
    {
        [SerializeField] private RenderTexture tex;
        [SerializeField] private Camera sceneCam;
        [SerializeField] private float baseOrthoSize;
        [SerializeField] private Vector2Int baseRes = new Vector2Int(1920, 1080);
        
        
        private void Update()
        {
            if (tex == null || sceneCam == null)
            {
                return;
            }
            
            tex.width = Screen.width;
            tex.height = Screen.height;
            
            sceneCam.orthographicSize = baseOrthoSize * Screen.height / baseRes.y;
        }

    }
}

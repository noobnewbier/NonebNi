using System;
using Unity.Mathematics;
using UnityEngine;
using UnityUtils;

namespace NonebNi.Develop
{
    //todo: https://discussions.unity.com/t/is-there-a-conversion-method-from-quaternion-to-euler/731052/30
    [ExecuteInEditMode]
    public class TRSTest : MonoBehaviour
    {
        [SerializeField] private Vector3 translation = Vector3.left;
        [SerializeField] private Vector3 rotAxis = Vector3.zero;
        [SerializeField] private Vector3 scale = Vector3.one;
        
        
        private void Update()
        {
            //todo: draw a bunch of lines - pointing from sphere to camera at all times. start with local space and convert to whatever space that needs to be.
            var mat = Matrix4x4.TRS(translation,  Quaternion.Euler(rotAxis), scale);
            
            var oTranslation = TransformUtils.GetTranslation(mat);
            var oRot = TransformUtils.GetRotation(mat);
            var oScale = TransformUtils.GetScale(mat);
            
            Debug.Log($"T {oTranslation}, {translation}");
            Debug.Log($"R {oRot}, {Quaternion.Euler(rotAxis)}");
            Debug.Log($"S {oScale}, {scale}");
            Debug.Log($"XY {oRot.x}:{oRot.y}, {math.EulerXYZ(oRot) * math.TODEGREES}");
        }
    }
}
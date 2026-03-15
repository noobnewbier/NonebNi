using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace NonebNi.Art.GrassInstancing
{
    public abstract class GrassInstancer
    {
        public abstract ComputeBuffer CreatePlacementBuffer(Bounds bounds);
        
        [SuppressMessage("ReSharper", "NotAccessedField.Local", Justification = "We use this in our shader code.")]
        protected struct InstanceData
        {
            public Matrix4x4 TRSMatrix;
        }
    }
}
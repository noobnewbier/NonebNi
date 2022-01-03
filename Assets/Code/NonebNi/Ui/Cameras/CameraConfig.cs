using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.Ui.Cameras
{
    [CreateAssetMenu(fileName = nameof(CameraConfig), menuName = MenuName.Data + nameof(CameraConfig))]
    public class CameraConfig : ScriptableObject
    {
        [Range(1, 1000f)] [SerializeField] private float maxPanningSpeed;
        [Range(0, 10f)] [SerializeField] private float bufferToClampingEdge;
        [Range(0f, 1f)] [SerializeField] private float edgePercentageToPan;
        [Range(1f, 5f)] [SerializeField] private float smoothFactor;
        [Range(0.1f, 1000f)] [SerializeField] private float maxZoomingSpeed;
        [Range(0.1f, 20f)] [SerializeField] private float minDistanceToMap;
        [Range(0.1f, 1000f)] [SerializeField] private float decelerationSpeed;
        [SerializeField] private bool isInvertedWheel;

        public float DecelerationSpeed => decelerationSpeed;
        public bool IsInvertedWheel => isInvertedWheel;
        public float MinDistanceToMap => minDistanceToMap;
        public float MaxZoomingSpeed => maxZoomingSpeed;
        public float BufferToClampingEdge => bufferToClampingEdge;
        public float EdgePercentageToPan => edgePercentageToPan;
        public float MaxPanningSpeed => maxPanningSpeed;
        public float SmoothFactor => smoothFactor;
    }
}
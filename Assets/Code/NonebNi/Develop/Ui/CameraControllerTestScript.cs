using NonebNi.Ui.Cameras;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtils.Editor;

namespace NonebNi.Develop
{
    public class CameraControllerTestScript : MonoBehaviour
    {
        [SerializeField] private float mapRadius;
        [FormerlySerializedAs("config"), SerializeField] private CameraControlSetting setting = null!;
        [SerializeField] private CinemachineCamera controlledCamera = null!;
        [SerializeField] private CinemachinePositionComposer composer = null!;

        [SerializeField] private GameObject lookAtObj1 = null!;
        [SerializeField] private GameObject lookAtObj2 = null!;

        private CameraController _cameraController = null!;

        private void Awake()
        {
            var (upBound, downBound, rightBound, leftBound) = GetCameraParameters();
            var config = new CameraConfig(setting, downBound, leftBound, rightBound, upBound);
            _cameraController = new CameraController(config, controlledCamera, composer);
        }

        private void Update()
        {
            _cameraController.UpdateCamera();
        }

        private void OnGUI()
        {
            var rect = new Rect(10, 10, 150, 25);

            if (GUI.Button(rect, "LookAt 1")) _cameraController.LookAt(lookAtObj1.transform.position);
            rect.y += 25;

            if (GUI.Button(rect, "LookAt 2")) _cameraController.LookAt(lookAtObj2.transform.position);
        }

        private void OnDrawGizmos()
        {
            //Too lazy to do this properly but if we need to change this more than 3 times we will refactor: current count=0
            using (new NonebEditorUtils.HandlesColorScope(Color.red))
            {
                // with our approach the y-size doesn't mean much does it...?
                var size = mapRadius * 2 - setting.BufferToClampingEdge * 2;
                var cubeSize = new Vector3(size, 1, size);
                Handles.DrawWireCube(Vector3.up * cubeSize.y * .5f, cubeSize);
            }
        }

        private (float upBound, float downBound, float rightBound, float leftBound) GetCameraParameters()
        {
            var mapMaxHeight = mapRadius;
            var mapMinHeight = -mapRadius;
            var mapMaxWidth = mapRadius;
            var mapMinWidth = -mapRadius;

            return (
                upBound: mapMaxHeight,
                downBound: mapMinHeight,
                rightBound: mapMaxWidth,
                leftBound: mapMinWidth
            );
        }
    }
}
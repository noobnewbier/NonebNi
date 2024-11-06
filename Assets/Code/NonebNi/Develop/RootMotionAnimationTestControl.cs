using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.CustomInspector;
using NonebNi.Ui.Common.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace NonebNi.Develop
{
    public class RootMotionAnimationTestControl : MonoBehaviour
    {
        [SerializeField] private GameObject groundObject = null!;
        [SerializeField] private Animator animator = null!;

        //Note: maybe root motion is not the best fit - we can just reverse engineer the speed via code to match the feet(this works well with speed blending as well?)
        [SerializeField] [AnimatorParameter(AnimatorControllerParameterType.Bool)]
        private string isWalkingFlag = null!;

        //Note: clip ref editor if we are using more of it in the future, for now it's good enough.
        [SerializeField] private AnimationClip clip = null!;

        private CancellationTokenSource? _cts;

        private Vector3 _targetPos;

        private void Awake()
        {
            _targetPos = transform.position;
        }

        private void Update()
        {
            ProcessInput();
            MoveToTargetPos();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(_targetPos, Vector3.one * 0.25f);
        }

        private void MoveToTargetPos()
        {
            if (!EditorData.instance.isWalkTillHitTarget) return;

            if (Vector3.Distance(transform.position, _targetPos) < 0.1f)
            {
                animator.enabled = false;
                return;
            }

            animator.enabled = true;
            transform.LookAt(_targetPos);
        }

        private void ProcessInput()
        {
            if (!Mouse.current.leftButton.isPressed) return;

            var (success, pos) = FindInputPos(Mouse.current.position);
            if (!success) return;

            _targetPos = pos;

            if (!EditorData.instance.isWalkTillHitTarget) WalkToTargetAndStop();
        }

        private async void WalkToTargetAndStop()
        {
            async UniTask Do(CancellationToken ct)
            {
                var selfTransform = transform;
                var speed = clip.averageSpeed.magnitude;
                var dist = Vector3.Distance(selfTransform.position, _targetPos);
                var walkDuration = dist / speed;

                selfTransform.LookAt(_targetPos);
                animator.SetBool(isWalkingFlag, true);

                ct.ThrowIfCancellationRequested();
                try
                {
                    await UniTask.WaitForSeconds(walkDuration, cancellationToken: ct);
                    animator.SetBool(isWalkingFlag, false);
                }
                catch (OperationCanceledException) { }
            }

            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            await Do(_cts.Token);
        }

        private (bool success, Vector3 pos) FindInputPos(Vector2Control mousePos)
        {
            var cam = Camera.main;
            if (cam == null) return default;

            var ray = cam.ScreenPointToRay(mousePos.ReadValue());
            if (!Physics.Raycast(ray, out var hit)) return default;

            if (hit.collider.gameObject != groundObject) return default;

            return (true, new Vector3(hit.point.x, transform.position.y, hit.point.z));
        }

        [CustomEditor(typeof(RootMotionAnimationTestControl))]
        private class InsideEditor : Editor
        {
            private NonebGUIDrawer _drawer = null!;

            private void OnEnable()
            {
                _drawer = new NonebGUIDrawer(new SerializedObject(EditorData.instance));
            }

            public override void OnInspectorGUI()
            {
                _drawer.Update();

                base.OnInspectorGUI();

                _drawer.DrawProperty(nameof(EditorData.isWalkTillHitTarget));
                _drawer.Apply();
            }
        }

        public class EditorData : ScriptableSingleton<EditorData>
        {
            public bool isWalkTillHitTarget;
        }
    }
}
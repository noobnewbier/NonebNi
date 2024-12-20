using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.CustomInspector;
using NonebNi.Ui.Animation.Common;
using NonebNi.Ui.Animation.MannequinFighter;
using NonebNi.Ui.Animation.Sequence;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace NonebNi.Develop
{
    public class MannequinAnimationTester : MonoBehaviour
    {
        [SerializeField] private GameObject groundObject = null!;
        [SerializeField] private MovementAnimationControl control = null!;
        [SerializeField] private MannequinFighterAnimationPlayer animPlayer = null!;


        private CancellationTokenSource? _cts;
        private Vector3 _targetPos;

        private void Awake()
        {
            _targetPos = transform.position;
        }

        private void Update()
        {
            ProcessInput();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(_targetPos, Vector3.one * 0.25f);
        }

        private void ProcessInput()
        {
            if (!Mouse.current.leftButton.wasReleasedThisFrame) return;

            var (success, pos) = FindInputPos(Mouse.current.position);
            if (!success) return;

            _targetPos = pos;

            WalkToTargetAndStop(_targetPos).Forget();
        }

        private async UniTask WalkToTargetAndStop(Vector3 targetPos)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();
            await control.WalkTo(_cts.Token, targetPos);
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

        private async UniTask PlayWrathStrikeAnimation()
        {
            await animPlayer.Play(new ApplyDamageAnimSequence("strike"));
        }

#if UNITY_EDITOR

        public class EditorData : ScriptableSingleton<EditorData> { }

        [CustomEditor(typeof(MannequinAnimationTester))]
        private class InsideEditor : Editor
        {
            private NonebGUIDrawer _drawer = null!;
            private MannequinAnimationTester _self = null!;

            private void OnEnable()
            {
                _drawer = new NonebGUIDrawer(new SerializedObject(EditorData.instance));
                _self = (MannequinAnimationTester)target;
            }

            public override void OnInspectorGUI()
            {
                _drawer.Update();

                base.OnInspectorGUI();

                if (_drawer.DrawButton(nameof(PlayWrathStrikeAnimation))) _self.PlayWrathStrikeAnimation();

                _drawer.Apply();
            }
        }

#endif
    }
}
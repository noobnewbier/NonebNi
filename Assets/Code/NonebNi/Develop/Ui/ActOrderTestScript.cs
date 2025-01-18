using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NonebNi.Core.Units;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using UnityEngine;

namespace NonebNi.Develop
{
    public class ActOrderTestScript : MonoBehaviour
    {
        [SerializeField] private UnitActOrderPanel panel = null!;
        [SerializeField] private NonebViewBehaviour view = null!;
        [SerializeField] private GameObject stackRoot = null!;

        private readonly Lazy<UnitData> _unitA = new(() => new UnitData("A"));
        private readonly Lazy<UnitData> _unitB = new(() => new UnitData("B"));
        private readonly Lazy<UnitData> _unitC = new(() => new UnitData("C"));
        private UIStack _stack;

        // ReSharper disable once Unity.IncorrectMethodSignature
        private async UniTaskVoid Start()
        {
            _stack = new UIStack(stackRoot);
            await _stack.Push(view);
        }

        private void OnGUI()
        {
            var rect = new Rect(10, 10, 150, 25);
            if (GUI.Button(rect, "Show ABC")) ShowPattern(_unitA.Value, _unitB.Value, _unitC.Value).Forget();
            rect.y += 25;

            if (GUI.Button(rect, "Show BCA")) ShowPattern(_unitB.Value, _unitC.Value, _unitA.Value).Forget();
            rect.y += 25;

            if (GUI.Button(rect, "Show and Cancel")) ShowAndCancel().Forget();
        }

        private async UniTask ShowAndCancel()
        {
            var cts = new CancellationTokenSource();
            var task = panel.Show(new[] { _unitA.Value, _unitB.Value, _unitC.Value }, cts.Token);
            cts.Cancel();

            await task;
        }

        private async UniTask ShowPattern(params UnitData[] units)
        {
            await panel.Show(units);
        }
    }
}
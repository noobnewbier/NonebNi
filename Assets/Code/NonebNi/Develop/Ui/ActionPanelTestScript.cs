using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.Element;
using Noneb.UI.View;
using NonebNi.Core.Actions;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using UnityEngine;

namespace NonebNi.Develop
{
    public class ActionPanelTestScript : MonoBehaviour
    {
        [SerializeField] private UnitActionPanel panel = null!;
        [SerializeField] private GameObject stackRoot = null!;

        private UIStack _stack = null!;

        // ReSharper disable once Unity.IncorrectMethodSignature
        private async UniTaskVoid Start()
        {
            var element = panel.FindOwner();
            var view = element!.OwnerView;
            _stack = new UIStack(stackRoot);

            await _stack.Push(view!);

            panel.ActionSelected += OnPanelOnActionSelected;
        }

        private void OnGUI()
        {
            var rect = new Rect(10, 10, 150, 25);
            if (GUI.Button(rect, "Show Control")) ShowPattern(true, ActionDatas.Bash, ActionDatas.Grapple, ActionDatas.Lure).Forget();
            rect.y += 25;

            if (GUI.Button(rect, "Show Inspect")) ShowPattern(false, ActionDatas.Bash, ActionDatas.Lure, ActionDatas.Grapple).Forget();
            rect.y += 25;

            if (GUI.Button(rect, "Show and Cancel")) ShowAndCancel().Forget();
        }

        private void OnPanelOnActionSelected(NonebAction? action)
        {
            panel.Select(action);
        }

        private async UniTask ShowAndCancel()
        {
            var cts = new CancellationTokenSource();
            var task = panel.Show(new[] { ActionDatas.Bash, ActionDatas.Grapple, ActionDatas.Lure }, true, cts.Token);
            cts.Cancel();

            await task;
        }

        private async UniTask ShowPattern(bool isInspect, params NonebAction[] actions)
        {
            await panel.Show(actions, isInspect);
        }
    }
}
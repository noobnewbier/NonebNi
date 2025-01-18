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
            if (GUI.Button(rect, "Show ABC")) ShowPattern(ActionDatas.Bash, ActionDatas.Grapple, ActionDatas.Lure).Forget();
            rect.y += 25;

            if (GUI.Button(rect, "Show BCA")) ShowPattern(ActionDatas.Bash, ActionDatas.Lure, ActionDatas.Grapple).Forget();
            rect.y += 25;

            if (GUI.Button(rect, "Show and Cancel")) ShowAndCancel().Forget();
        }

        private void OnPanelOnActionSelected(NonebAction? action)
        {
            async UniTaskVoid Do()
            {
                await panel.Highlight(action);
            }

            Do().Forget();
        }

        private async UniTask ShowAndCancel()
        {
            var cts = new CancellationTokenSource();
            var task = panel.Show(new[] { ActionDatas.Bash, ActionDatas.Grapple, ActionDatas.Lure }, cts.Token);
            cts.Cancel();

            await task;
        }

        private async UniTask ShowPattern(params NonebAction[] actions)
        {
            await panel.Show(actions);
        }
    }
}
using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NonebNi.Ui.Cameras;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using UnityEngine;

namespace NonebNi.Main
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] private PlayerTurnMenu playerTurnMenu = null!;

        private UIStack _stack = null!;

        public void Init(IPlayerTurnPresenter playerTurnPresenter, IPlayerTurnWorldSpaceInputControl worldSpaceInputControl, ICameraController cameraController)
        {
            _stack = new UIStack(gameObject);
            playerTurnMenu.Init(playerTurnPresenter, worldSpaceInputControl, cameraController);

            //todo: need to await
            _stack.Push(playerTurnMenu).Forget();
        }
    }
}
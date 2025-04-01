using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NonebNi.Core.Agents;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Maps;
using NonebNi.Terrain;
using NonebNi.Ui.Cameras;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using UnityEngine;

namespace NonebNi.Main
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] private PlayerTurnMenu playerTurnMenu = null!;

        private UIStack _stack = null!;

        public void Init(
            IPlayerTurnWorldSpaceInputControl worldSpaceInputControl,
            ICameraController cameraController,
            IPlayerAgent agent,
            ICoordinateAndPositionService coordinateAndPositionService,
            IReadOnlyMap map,
            IUnitTurnOrderer unitTurnOrderer)
        {
            _stack = new UIStack(gameObject);
            playerTurnMenu.Init(
                worldSpaceInputControl,
                cameraController,
                agent,
                coordinateAndPositionService,
                map,
                unitTurnOrderer
            );

            //todo: need to await
            _stack.Push(playerTurnMenu).Forget();
        }
    }
}
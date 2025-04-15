using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NonebNi.Core.Agents;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Maps;
using NonebNi.Core.Units;
using NonebNi.Terrain;
using NonebNi.Ui.Attributes;
using NonebNi.Ui.Cameras;
using NonebNi.Ui.ViewComponents.EnemyTurnMenu;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using UnityEngine;

namespace NonebNi.Main
{
    [NonebUniversalEditor]
    public class Hud : MonoBehaviour
    {
        [SerializeField] private PlayerTurnMenu playerTurnMenu = null!;
        [SerializeField] private EnemyTurnMenu enemyTurnMenu = null!;
        private IPlayerAgent _agent = null!;

        private UIStack _stack = null!;

        public void Init(
            IPlayerTurnWorldSpaceInputControl worldSpaceInputControl,
            ICameraController cameraController,
            IPlayerAgent agent,
            ICoordinateAndPositionService coordinateAndPositionService,
            IReadOnlyMap map,
            IUnitTurnOrderer unitTurnOrderer)
        {
            _agent = agent;
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

        public void RefreshForNewTurn(UnitData currentUnit)
        {
            //todo: cts support?
            async UniTask Do()
            {
                if (currentUnit.FactionId == _agent.Faction.Id)
                {
                    if (!_stack.IsCurrentComponent(playerTurnMenu))
                        await _stack.ReplaceCurrent(playerTurnMenu);
                    else
                        await playerTurnMenu.ShowCurrentTurnUnit();
                }
                else
                {
                    await _stack.ReplaceCurrent(enemyTurnMenu);
                }
            }

            Do().Forget();
        }
    }
}
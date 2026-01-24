using Noneb.UI.View;
using NonebNi.Terrain;
using NonebNi.Ui.Cameras;
using UnityEngine;

namespace NonebNi.Ui.ViewComponents.EnemyTurnMenu
{
    public interface IEnemyTurnMenu : IViewComponent { }

    public class EnemyTurnMenu : MonoBehaviour, IEnemyTurnMenu
    {
        private ICameraController _cameraController = null!;
        private ICoordinateAndPositionService _coordinateAndPositionService = null!;

        public void Init(
            ICameraController cameraController,
            ICoordinateAndPositionService coordinateAndPositionService)
        {
            _cameraController = cameraController;
            _coordinateAndPositionService = coordinateAndPositionService;
        }
    }
}
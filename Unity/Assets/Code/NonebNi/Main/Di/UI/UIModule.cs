using Noneb.UI.InputSystems;
using NonebNi.Terrain;
using NonebNi.Ui.Grids;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using StrongInject;

namespace NonebNi.Main.Di.UI
{
    [
        Register(typeof(LevelUi), typeof(ILevelUi)),
        Register(typeof(DecisionFlowControl), typeof(IDecisionFlowControl)),
        Register(typeof(PlayerTurnWorldSpaceInputControl), typeof(IPlayerTurnWorldSpaceInputControl)),
        Register(typeof(NonebInputSystem), typeof(IInputSystem)),
        Register(typeof(TerrainMeshCreator), typeof(ITerrainMeshCreator)),
        Register(typeof(HexHighlighter), typeof(IHexHighlighter)),
        Register(typeof(CoordinateAndPositionService), typeof(ICoordinateAndPositionService)),
        RegisterModule(typeof(CameraControllerModule)),
        RegisterModule(typeof(HudModule)),
        RegisterModule(typeof(SharedContextModule))
    ]
    public class UIModule { }
}
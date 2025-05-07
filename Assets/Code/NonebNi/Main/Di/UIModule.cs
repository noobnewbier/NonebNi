using Noneb.UI.InputSystems;
using NonebNi.Ui.Grids;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using StrongInject;

namespace NonebNi.Main.Di
{
    //TODO: change the DI
    [
        Register(typeof(ActionInputControl), typeof(IActionInputControl)),
        Register(typeof(PlayerTurnWorldSpaceInputControl), typeof(IPlayerTurnWorldSpaceInputControl)),
        Register(typeof(NonebInputSystem), typeof(IInputSystem)),
        Register(typeof(HexHighlighter), typeof(IHexHighlighter)),
        RegisterModule(typeof(HudModule))
    ]
    public class UIModule { }
}
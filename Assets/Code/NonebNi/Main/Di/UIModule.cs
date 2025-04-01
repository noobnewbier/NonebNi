using Noneb.UI.InputSystems;
using NonebNi.Ui.Grids;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using StrongInject;

namespace NonebNi.Main.Di
{
    //TODO: change the DI
    [Register(typeof(PlayerTurnMenu), typeof(IPlayerTurnMenu)), Register(typeof(PlayerTurnWorldSpaceInputControl), typeof(IPlayerTurnWorldSpaceInputControl)), Register(typeof(NonebInputSystem), typeof(IInputSystem)), Register(typeof(HexHighlighter), typeof(IHexHighlighter))]
    // [Register(typeof(HexHighlightConfig),typeof()]
    // [Register(typeof(Core.Maps.Map),typeof()]
    // [Register(typeof(UnityEngine.Camera),typeof()]
    public class UIModule { }
}
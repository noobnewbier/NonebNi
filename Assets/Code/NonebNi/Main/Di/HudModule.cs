using NonebNi.Ui.ViewComponents.Combos;
using NonebNi.Ui.ViewComponents.HexTooltip;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using StrongInject;

namespace NonebNi.Main.Di
{
    [
        Register(typeof(Hud.Dependencies)),
        Register(typeof(PlayerTurnMenu.Dependencies)),
        Register(typeof(ComboActionSelectionMenu.Dependencies)),
        Register(typeof(ComboUnitSelectionMenu.Dependencies)),
        Register(typeof(HexTooltipControl.Dependencies))
    ]
    public class HudModule { }
}
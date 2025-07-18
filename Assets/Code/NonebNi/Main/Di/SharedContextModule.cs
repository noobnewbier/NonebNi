using NonebNi.Ui.Tooltips;
using NonebNi.Ui.UIContexts;
using NonebNi.Ui.ViewComponents.HexTooltip;
using StrongInject;
using StrongInject.Modules;

namespace NonebNi.Main.Di
{
    [
        RegisterModule(typeof(CollectionsModule)),
        Register(typeof(SharedContextInitializer)),
        Register(typeof(TooltipDetector.Dependencies), typeof(ISharedInContext)),
        Register(typeof(TooltipCanvas.Dependencies), typeof(ISharedInContext)),
        Register(typeof(HexTooltipControl.Dependencies), typeof(ISharedInContext))
    ]
    public class SharedContextModule { }
}
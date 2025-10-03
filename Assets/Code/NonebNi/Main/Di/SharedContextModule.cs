using NonebNi.Core.AI;
using NonebNi.Core.GameContexts;
using NonebNi.Ui.Inputs;
using NonebNi.Ui.Tooltips;
using NonebNi.Ui.ViewComponents.HexTooltip;
using StrongInject;
using StrongInject.Modules;

namespace NonebNi.Main.Di
{
    //todo: what to do with this?
    [
        RegisterModule(typeof(CollectionsModule)),
        Register(typeof(SharedContextInitializer)),
        Register(typeof(TooltipDetector.Dependencies), typeof(object)),
        Register(typeof(TooltipCanvas.Dependencies), typeof(object)),
        Register(typeof(HexTooltipControl.Dependencies), typeof(object)),
        Register(typeof(ZeroVector2IfOverUIProcessor.Dependencies), typeof(object)),
        Register(typeof(DamageOpponentNode.Dependencies), typeof(object))
    ]
    public class SharedContextModule { }
}
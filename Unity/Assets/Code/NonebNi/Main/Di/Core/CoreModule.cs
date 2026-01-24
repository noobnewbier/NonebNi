using NonebNi.Core.AI;
using StrongInject;

namespace NonebNi.Main.Di.Core
{
    [
        Register(typeof(InfluenceMap), typeof(IInfluenceMap)),
        RegisterModule(typeof(GameSaveModule)),
        RegisterModule(typeof(LevelFlowControlModule))
    ]
    public class CoreModule { }
}
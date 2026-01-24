using NonebNi.Core.Debug;
using NonebNi.Ui.Debug;
using StrongInject;

namespace NonebNi.Main.Di.Debug
{
    /*
     * Note:
     * I haven't bother with extracting the interface atm.
     * We can worry about that later, where we will be at a point we can regret about all our life decisions.
     */
    [
        Register(typeof(DebugTools)),
        RegisterModule(typeof(NonebDebugConsoleModule)),
        /*
         * If these lots grow we can declare individual modules,
         * but for now it's overengineering to create their own.
         */
        Register(typeof(InfluenceMapVisualizer)),
        Register(typeof(DebugFlagRepository), typeof(IDebugFlagRepository))
    ]
    public class DebugModule { }
}
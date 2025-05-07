using System.Linq;
using NonebNi.Core.Agents;
using StrongInject;

namespace NonebNi.Main.Di
{
    [
        Register(typeof(AgentsService), typeof(IAgentsService))
    ]
    public class AgentsModule
    {
        [Factory]
        public static IPlayerAgent ProvidePlayerAgent(IAgent[] agents) => agents.OfType<IPlayerAgent>().First();
    }
}
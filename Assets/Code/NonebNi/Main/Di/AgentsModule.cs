using System.Linq;
using NonebNi.Core.Agents;
using StrongInject;

namespace NonebNi.Main.Di
{
    public class AgentsModule
    {
        [Factory]
        public static IPlayerAgent ProvidePlayerAgent(IAgent[] agents) => agents.OfType<IPlayerAgent>().First();

        [Factory]
        public static IAgentsService ProvideAgentService(IAgent[] agents) => new AgentsService(agents);
    }
}
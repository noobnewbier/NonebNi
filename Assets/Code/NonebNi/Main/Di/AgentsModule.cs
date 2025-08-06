using System.Linq;
using NonebNi.Core.Agents;
using NonebNi.Core.Factions;
using StrongInject;

namespace NonebNi.Main.Di
{
    [
        //don't really like naming stuffs "service" but we will stick with it for now.
        Register(typeof(AgentsService), typeof(IAgentsService)),
        Register(typeof(FactionService), typeof(IFactionService))
    ]
    public class AgentsModule
    {
        [Factory]
        public static IPlayerAgent ProvidePlayerAgent(IAgent[] agents) => agents.OfType<IPlayerAgent>().First();
    }
}
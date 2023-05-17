using System.Linq;
using NonebNi.Core.Agents;
using NonebNi.Core.Factions;
using StrongInject;

namespace NonebNi.Main.Di
{
    public class AgentsModule
    {
        [Instance] public static IAgent[] _agents =
        {
            new PlayerAgent(FactionsData.Player.Id),
            new DummyAgent(FactionsData.EnemyNpc.Id)
        };

        [Factory]
        public static IPlayerAgent ProvidePlayerAgent() => _agents.OfType<IPlayerAgent>().First();

        [Factory]
        public static IAgentsService ProvideAgentService(IAgent[] agents) => new AgentsService(agents);
    }
}
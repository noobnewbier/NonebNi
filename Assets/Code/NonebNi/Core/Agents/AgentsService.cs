using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Decision;
using UnityEngine;

namespace NonebNi.Core.Agents
{
    public interface IAgentsService
    {
        UniTask<IDecision?> GetAgentDecision(string factionId);

        void OverrideDecision(IDecision decision);
    }

    public class AgentsService : IAgentsService
    {
        private readonly IReadOnlyList<IAgent> _agents;

        private CancellationTokenSource? _getDecisionCts;
        private IDecision? _overridingDecision;

        public AgentsService(IReadOnlyList<IAgent> agents)
        {
            if (agents.GroupBy(a => a.FactionId).Any(g => g.Count() > 1))
            {
                Debug.LogError("More than one agent shares the same faction id - this is invalid");
            }

            _agents = agents;
        }

        public async UniTask<IDecision?> GetAgentDecision(string factionId)
        {
            //resetting the value so the last value won't carry forward to this call
            _getDecisionCts = new CancellationTokenSource();
            _overridingDecision = null;

            var agent = _agents.FirstOrDefault(a => a.FactionId == factionId);
            if (agent == null)
            {
                return null;
            }

            var ct = _getDecisionCts.Token;
            var agentDecision = await agent.GetDecision(ct);

            return _overridingDecision ?? agentDecision;
        }

        public void OverrideDecision(IDecision decision)
        {
            if (_getDecisionCts == null)
            {
                //not trying to get a decision atm -> nothing to override.
                return;
            }

            _overridingDecision = decision;
            _getDecisionCts.Cancel();
        }
    }
}
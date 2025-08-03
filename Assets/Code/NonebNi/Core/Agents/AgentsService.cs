using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Commands;
using NonebNi.Core.Decisions;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Core.Agents
{
    public interface IAgentsService
    {
        void OverrideDecision(IDecision decision);
        UniTask<ICommand> GetAgentInput(string factionId);
    }

    public class AgentsService : IAgentsService
    {
        private readonly IReadOnlyList<IAgent> _agents;
        private readonly IDecisionValidator _validator;

        private CancellationTokenSource? _getDecisionCts;
        private IDecision? _overridingDecision;

        public AgentsService(IReadOnlyList<IAgent> agents, IDecisionValidator validator)
        {
            if (agents.GroupBy(a => a.Faction).Any(g => g.Count() > 1))
                Debug.LogError("More than one agent shares the same faction id - this is invalid");

            _agents = agents;
            _validator = validator;
        }

        public async UniTask<ICommand> GetAgentInput(string factionId)
        {
            IDecisionValidator.Error? err;
            ICommand command;
            do
            {
                var decision = await GetAgentDecision(factionId);
                Log.Info($"[Level] Received Decision: {decision?.GetType()}");

                (err, command) = _validator.ValidateDecision(decision);

                if (err != null) Log.Info($"[Level] Decision Error: {err.Type}, {err.Description}");
            } while (err != null);

            Log.Info($"[Level] Evaluate Command: {command.GetType()}");

            //TODO: preferrably there is an auto end turn button which ends turn when there's absolutely nothing you can do, hard to implement without being annoying though
            return command;
        }

        public void OverrideDecision(IDecision decision)
        {
            if (_getDecisionCts == null)
                //not trying to get a decision atm -> nothing to override.
                return;

            _overridingDecision = decision;
            _getDecisionCts.Cancel();
        }

        private async UniTask<IDecision?> GetAgentDecision(string factionId)
        {
            //resetting the value so the last value won't carry forward to this call
            _getDecisionCts = new CancellationTokenSource();
            _overridingDecision = null;

            var agent = _agents.FirstOrDefault(a => a.Faction.Id == factionId);
            if (agent == null) return null;

            var ct = _getDecisionCts.Token;
            var agentDecision = await agent.GetDecision(ct);

            return _overridingDecision ?? agentDecision;
        }
    }
}
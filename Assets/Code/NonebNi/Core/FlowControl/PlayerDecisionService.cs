using Cysharp.Threading.Tasks;
using NonebNi.Core.Decision;

namespace NonebNi.Core.FlowControl
{
    public interface IAgentDecisionService
    {
        void SetDecision(IDecision decision);
        UniTask<IDecision> GetNextDecision();
    }

    public class AgentDecisionService : IAgentDecisionService
    {
        private IDecision? _currentDecision;

        public void SetDecision(IDecision decision)
        {
            _currentDecision = decision;
        }

        public async UniTask<IDecision> GetNextDecision()
        {
            //Resetting the previous value
            _currentDecision = null;

            await UniTask.WaitUntil(() => _currentDecision != null);

            return _currentDecision!;
        }
    }
}
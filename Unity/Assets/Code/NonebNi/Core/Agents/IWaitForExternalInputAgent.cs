using NonebNi.Core.Decisions;

namespace NonebNi.Core.Agents
{
    public interface IWaitForExternalInputAgent : IAgent
    {
        //todo: eventually maybe there should be a "brain"(ai brain, vs player brain), and we can only have one type of agent which would be nice 
        void SetDecision(IDecision decision);
    }
}
using NonebNi.Core.Entities;

namespace NonebNi.Core.Decisions
{
    public class InspectDecision : IDecision
    {
        public readonly EntityData ToInspect;

        public InspectDecision(EntityData toInspect)
        {
            ToInspect = toInspect;
        }
    }
}
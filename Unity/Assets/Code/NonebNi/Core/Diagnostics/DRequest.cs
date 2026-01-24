using NonebNi.Core.Coordinates;

namespace NonebNi.Core.Diagnostics
{
    public abstract record DRequest
    {
        public record EngageUtility(Coordinate Coordinate, float DistUtil, float OutOfLinePenalty, float CrossLanePenalty) : DRequest;
    }
}
using NonebNi.Core.Coordinates;

namespace NonebNi.Core.AI
{
    public interface IInfluenceSource
    {
        //Note: this can use formula as well?
        float FindInfluence(Coordinate coord);
        bool IsValid();
    }
}
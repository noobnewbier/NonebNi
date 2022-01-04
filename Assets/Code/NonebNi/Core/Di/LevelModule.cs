using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;

namespace NonebNi.Core.Di
{
    public class LevelModule
    {
        public LevelData LevelData { get; }

        public LevelModule(LevelData levelData)
        {
            LevelData = levelData;
        }

        public ILevelFlowController GetLevelFlowController(ICommandEvaluationService commandEvaluationService) =>
            new LevelFlowController(commandEvaluationService, GetUnitTurnOrderer());

        private IUnitTurnOrderer GetUnitTurnOrderer() => new UnitTurnOrderer(LevelData.Map);
    }
}
using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;
using NonebNi.Core.Sequences;
using NonebNi.Ui.Entities;
using NonebNi.Ui.Sequences;

namespace NonebNi.Main.Di
{
    public class LevelModule
    {
        public LevelData LevelData { get; }
        public IEntityRepository EntityRepository { get; }

        public LevelModule(LevelData levelData)
        {
            LevelData = levelData;
            EntityRepository = new EntityRepository();
        }

        public ILevelFlowController GetLevelFlowController(ICommandEvaluationService commandEvaluationService,
                                                           IUnitTurnOrderer unitTurnOrderer,
                                                           IPlayerDecisionService playerDecisionService,
                                                           ISequencePlayer sequencePlayer) =>
            new LevelFlowController(
                commandEvaluationService,
                unitTurnOrderer,
                playerDecisionService,
                sequencePlayer
            );

        public IPlayerDecisionService GetPlayerDecisionService() => new PlayerDecisionService();

        public IUnitTurnOrderer GetUnitTurnOrderer() => new UnitTurnOrderer(LevelData.Map);

        public ICommandEvaluationService GetCommandEvaluationService() => new CommandEvaluationService(LevelData.Map);

        public ISequencePlayer GetSequencePlayer() => new SequencePlayer(EntityRepository);
    }
}
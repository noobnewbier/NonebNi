﻿using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;
using NonebNi.Core.Sequences;
using NonebNi.Ui.Sequences;

namespace NonebNi.Main.Di
{
    public class LevelModule
    {
        public LevelData LevelData { get; }

        public LevelModule(LevelData levelData)
        {
            LevelData = levelData;
        }

        public ILevelFlowController GetLevelFlowController() =>
            new LevelFlowController(
                GetCommandEvaluationService(),
                GetUnitTurnOrderer(),
                GetPlayerDecisionService(),
                GetSequencePlayer()
            );

        private IPlayerDecisionService GetPlayerDecisionService() => new PlayerDecisionService();

        private IUnitTurnOrderer GetUnitTurnOrderer() => new UnitTurnOrderer(LevelData.Map);

        private ICommandEvaluationService GetCommandEvaluationService() => new CommandEvaluationService(LevelData.Map);

        private ISequencePlayer GetSequencePlayer() => new SequencePlayer();
    }
}
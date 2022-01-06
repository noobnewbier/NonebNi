using System.Collections.Generic;
using NonebNi.Core.StateMachines;

namespace NonebNi.Core.FlowControl
{
    public interface ILevelFlowController
    {
        void UpdateState();
        void EndTurn();
        void FinishEvaluation();
    }

    public class LevelFlowController : ILevelFlowController
    {
        private readonly StateMachine _stateMachine;

        public LevelFlowController(ICommandEvaluationService evaluationService,
                                   IUnitTurnOrderer unitTurnOrderer,
                                   IPlayerDecisionService playerDecisionService)
        {
            var decisionState = new DecisionState(unitTurnOrderer);
            var evaluationState = new EvaluationState(evaluationService, this, playerDecisionService);
            var endState = new EndState();

            _stateMachine = new StateMachine(decisionState);
            _stateMachine.AddTransition(
                decisionState,
                new Transition(new HashSet<string> { Params.EndTurn }, evaluationState)
            );
            _stateMachine.AddTransition(
                evaluationState,
                new Transition(new HashSet<string> { Params.EndGame }, endState),
                new Transition(new HashSet<string> { Params.NextTurn }, decisionState)
            );
        }

        public void UpdateState()
        {
            _stateMachine.UpdateState();
        }

        public void EndTurn()
        {
            _stateMachine.SetTrigger(Params.EndTurn);
        }

        public void FinishEvaluation()
        {
            _stateMachine.SetTrigger(Params.NextTurn);
        }

        private static class Params
        {
            public const string EndTurn = nameof(EndTurn);
            public const string NextTurn = nameof(NextTurn);
            public const string EndGame = nameof(EndGame);
        }
    }
}
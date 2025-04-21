using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Agents;
using NonebNi.Core.Combos;
using NonebNi.Core.Commands;
using NonebNi.Core.Decisions;
using NonebNi.Core.Sequences;
using NonebNi.Core.Units;
using Unity.Logging;

namespace NonebNi.Core.FlowControl
{
    public interface ILevelFlowController
    {
        delegate void TurnStarted(UnitData currentUnit);

        IAgentsService AgentsService { get; }
        IActionCommandEvaluator Evaluator { get; }
        ISequencePlayer SequencePlayer { get; }
        IUnitTurnOrderer UnitTurnOrderer { get; }
        UniTask Run();
        event TurnStarted NewTurnStarted;
    }

    public class LevelFlowController : ILevelFlowController
    {
        public LevelFlowController(
            IActionCommandEvaluator evaluator,
            IUnitTurnOrderer unitTurnOrderer,
            IAgentsService agentService,
            ISequencePlayer sequencePlayer,
            IDecisionValidator decisionValidator,
            IComboChecker comboChecker)
        {
            Evaluator = evaluator;
            UnitTurnOrderer = unitTurnOrderer;
            AgentsService = agentService;
            SequencePlayer = sequencePlayer;
            DecisionValidator = decisionValidator;
            _comboChecker = comboChecker;
        }

        public IDecisionValidator DecisionValidator { get; }

        public IAgentsService AgentsService { get; }

        public IActionCommandEvaluator Evaluator { get; }

        public ISequencePlayer SequencePlayer { get; }

        public IUnitTurnOrderer UnitTurnOrderer { get; }

        private readonly IComboChecker _comboChecker;

        public async UniTask Run()
        {
            //TODO: replace all these logging w/ a Decorator using StrongInject.

            var turnNum = 0; //Mostly for debug purposes - but probably necessary for UI at some point
            while (true)
            {
                var currentUnit = UnitTurnOrderer.CurrentUnit;
                Log.Info($"[Level] Turn {turnNum}, {currentUnit.Name}'s turn");

                currentUnit.RestoreMovement();
                currentUnit.RestoreActionPoint();
                currentUnit.RecoverFatigue();

                NewTurnStarted?.Invoke(currentUnit);

                while (true)
                {
                    // ReSharper restore RedundantAssignment
                    var command = await WaitForAgencyInput(currentUnit);
                    bool isDone;
                    switch (command)
                    {
                        case ActionCommand actionCommand:
                            await ActionEvaluationFlow(actionCommand, currentUnit);
                            isDone = false;
                            break;
                        case EndTurnCommand:
                        case NullCommand:
                            isDone = true;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(command));
                    }

                    if (isDone) break;
                }

                turnNum++;
                UnitTurnOrderer.ToNextUnit();
            }

            // Expected, this should just run forever, until we have a exit/win/lose condition
            // ReSharper disable once FunctionNeverReturns
        }

        public event ILevelFlowController.TurnStarted? NewTurnStarted;

        private async UniTask ActionEvaluationFlow(ActionCommand command, UnitData currentUnit)
        {
            var startActionSequence = Evaluator.Evaluate(command).ToArray();
            var comboSequence = (await GetComboSequence(command, currentUnit)).ToArray();
            var fullSequence = startActionSequence.Concat(comboSequence);
            await SequencePlayer.Play(fullSequence);

            Log.Info("[Level] Finished Evaluation");
        }

        //todo: combo checker - how is it used? that idk but we will find out. I think it might be a good idea to have core to signal that we need input from UI and have the UI act accordingly.
        private async UniTask<IEnumerable<ISequence>> GetComboSequence(ActionCommand previousCommand, UnitData comboStarter)
        {
            if (!previousCommand.Action.Effects.Any())
            {
                //todo: combo effect, or effect defining if they are comboable. or even action?
            }

//todo: somewhere somehow, the type hierarchy has gone wrong, very wrong
            if (await WaitForAgencyInput(comboStarter) is not ActionCommand actionCommand) return Enumerable.Empty<ISequence>();

            if (actionCommand.ActorEntity is not UnitData comboTaker) return Enumerable.Empty<ISequence>();

            var sequence = Evaluator.Evaluate(actionCommand).ToArray();
            var nextSequence = await GetComboSequence(actionCommand, comboTaker);

            return sequence.Concat(nextSequence);
        }

        private async UniTask<ICommand> WaitForAgencyInput(UnitData currentUnit)
        {
            IDecisionValidator.Error? err;
            ICommand command;
            do
            {
                var decision = await AgentsService.GetAgentDecision(currentUnit.FactionId);
                Log.Info($"[Level] Received Decision: {decision?.GetType()}");

                (err, command) = DecisionValidator.ValidateDecision(decision);

                if (err != null) Log.Info($"[Level] Decision Error: {err.Id}, {err.Description}");
            } while (err != null);

            Log.Info($"[Level] Evaluate Command: {command.GetType()}");

            //TODO: preferrably there is an auto end turn button which ends turn when there's absolutely nothing you can do, hard to implement without being annoying though
            return command;
        }
    }
}
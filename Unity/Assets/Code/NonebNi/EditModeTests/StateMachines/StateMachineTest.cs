using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NonebNi.Core.StateMachines;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace NonebNi.EditModeTests.StateMachines
{
    public class StateMachineTest
    {
        [UnityTest]
        public IEnumerator StateMachineWithOnlyOneState_TickStateMachine_DefaultStateIsUsed()
        {
            var defaultStateMock = new Mock<IState>();
            var stateMachine = new StateMachine(defaultStateMock.Object);

            yield return stateMachine.UpdateState();

            defaultStateMock.Verify(s => s.OnEnterState(), Times.Once);
        }

        [UnityTest]
        public IEnumerator UpdateState_ForTheFirstTime_OnExitForThePreviousStateIsCalled()
        {
            var defaultStateMock = new Mock<IState>();
            var otherStateMock = new Mock<IState>();
            var stateMachine = new StateMachine(defaultStateMock.Object);
            stateMachine.AddTransition(
                defaultStateMock.Object,
                new Transition(new HashSet<string> { TestParams.ToOtherState }, otherStateMock.Object)
            );
            yield return stateMachine.UpdateState();
            yield return null;

            stateMachine.SetTrigger(TestParams.ToOtherState);
            yield return stateMachine.UpdateState();
            yield return null;

            defaultStateMock.Verify(s => s.OnExitState(), Times.Once);
        }

        [UnityTest]
        public IEnumerator StateMachineWithOnlyOneState_TickStateMachineTwice_UpdateIsCalledOnce()
        {
            var defaultStateMock = new Mock<IState>();
            var stateMachine = new StateMachine(defaultStateMock.Object);

            yield return stateMachine.UpdateState();
            yield return stateMachine.UpdateState();

            defaultStateMock.Verify(s => s.OnUpdate(), Times.Once);
        }

        [UnityTest]
        public IEnumerator StateMachineWithTransition_NoParametersAreSet_DefaultStateIsUsed()
        {
            var defaultStateMock = new Mock<IState>();
            var otherStateMock = new Mock<IState>();
            var stateMachine = new StateMachine(defaultStateMock.Object);
            stateMachine.AddTransition(
                defaultStateMock.Object,
                new Transition(new HashSet<string> { TestParams.ToOtherState }, otherStateMock.Object)
            );

            yield return stateMachine.UpdateState();

            defaultStateMock.Verify(s => s.OnEnterState(), Times.Once);
            otherStateMock.Verify(s => s.OnEnterState(), Times.Never);
        }

        [UnityTest]
        public IEnumerator WithPossibleTransitionDefinedAtFirstFrame_TickOnlyOnce_DefaultStateIsUsed()
        {
            var defaultStateMock = new Mock<IState>();
            var otherStateMock = new Mock<IState>();
            var stateMachine = new StateMachine(defaultStateMock.Object);
            stateMachine.AddTransition(
                defaultStateMock.Object,
                new Transition(new HashSet<string> { TestParams.ToOtherState }, otherStateMock.Object)
            );

            stateMachine.SetTrigger(TestParams.ToOtherState);
            yield return stateMachine.UpdateState();

            otherStateMock.Verify(s => s.OnEnterState(), Times.Never);
            defaultStateMock.Verify(s => s.OnEnterState(), Times.Once);
        }

        [UnityTest]
        public IEnumerator WithPossibleTransitionDefinedAtFirstFrame_TickTwice_MoveToOtherState()
        {
            var defaultStateMock = new Mock<IState>();
            var otherStateMock = new Mock<IState>();
            var stateMachine = new StateMachine(defaultStateMock.Object);
            stateMachine.AddTransition(
                defaultStateMock.Object,
                new Transition(new HashSet<string> { TestParams.ToOtherState }, otherStateMock.Object)
            );

            stateMachine.SetTrigger(TestParams.ToOtherState);
            yield return stateMachine.UpdateState();
            yield return stateMachine.UpdateState();

            otherStateMock.Verify(s => s.OnEnterState(), Times.Once);
            defaultStateMock.Verify(s => s.OnEnterState(), Times.Once);
            defaultStateMock.Verify(s => s.OnExitState(), Times.Once);
        }

        [UnityTest]
        public IEnumerator GivenMultiplePossibleTransitions_OneWithHighestPriorityIsUsed()
        {
            var defaultStateMock = new Mock<IState>();
            var lowerPriorityStateMock = new Mock<IState>();
            var higherPriorityStateMock = new Mock<IState>();
            var stateMachine = new StateMachine(defaultStateMock.Object);
            stateMachine.AddTransition(
                defaultStateMock.Object,
                new Transition(new HashSet<string> { TestParams.ToLowerState }, lowerPriorityStateMock.Object, 1)
            );
            stateMachine.AddTransition(
                defaultStateMock.Object,
                new Transition(new HashSet<string> { TestParams.ToHigherState }, higherPriorityStateMock.Object)
            );

            stateMachine.SetTrigger(TestParams.ToLowerState);
            stateMachine.SetTrigger(TestParams.ToHigherState);
            yield return stateMachine.UpdateState();
            yield return stateMachine.UpdateState();

            higherPriorityStateMock.Verify(s => s.OnEnterState(), Times.Once);
            lowerPriorityStateMock.Verify(s => s.OnEnterState(), Times.Never);
        }

        [UnityTest]
        public IEnumerator GivenMultiplePossibleTransitionsWithSamePriority_OneWithMoreParametersIsUsed()
        {
            var defaultStateMock = new Mock<IState>();
            var lowerPriorityStateMock = new Mock<IState>();
            var higherPriorityStateMock = new Mock<IState>();
            var stateMachine = new StateMachine(defaultStateMock.Object);
            stateMachine.AddTransition(
                defaultStateMock.Object,
                new Transition(new HashSet<string> { TestParams.ToOtherState }, lowerPriorityStateMock.Object)
            );
            stateMachine.AddTransition(
                defaultStateMock.Object,
                new Transition(
                    new HashSet<string> { TestParams.ToOtherState, TestParams.ExtraParamToOtherState },
                    higherPriorityStateMock.Object
                )
            );

            stateMachine.SetTrigger(TestParams.ToOtherState);
            stateMachine.SetTrigger(TestParams.ExtraParamToOtherState);
            yield return stateMachine.UpdateState();
            yield return stateMachine.UpdateState();

            higherPriorityStateMock.Verify(s => s.OnEnterState(), Times.Once);
            lowerPriorityStateMock.Verify(s => s.OnEnterState(), Times.Never);
        }

        [Test]
        public void SetParameterThatNoTransitionUses_InvalidOperationExceptionIsThrown()
        {
            var defaultStateMock = new Mock<IState>();
            var stateMachine = new StateMachine(defaultStateMock.Object);

            Assert.Throws<InvalidOperationException>(() => stateMachine.SetTrigger(TestParams.NonExistingParam));
        }

        private static class TestParams
        {
            public const string ToOtherState = "TO_OTHER_STATE";
            public const string ExtraParamToOtherState = "EXTRA_PARAM_TO_OTHER_STATE";
            public const string ToLowerState = "TO_LOWER_STATE";
            public const string ToHigherState = "TO_HIGHER_STATE";
            public const string NonExistingParam = "NON_EXISTING_PARAM";
        }
    }
}
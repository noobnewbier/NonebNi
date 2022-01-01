using System;
using System.Collections.Generic;
using Moq;
using NonebNi.Core.StateMachines;
using NUnit.Framework;

namespace NonebNi.EditModeTests.StateMachines
{
    public class StateMachineTest
    {
        [Test]
        public void StateMachineWithOnlyOneState_TickStateMachine_DefaultStateIsUsed()
        {
            var defaultStateMock = new Mock<IState>();
            var stateMachine = new StateMachine(defaultStateMock.Object);

            stateMachine.Tick();

            defaultStateMock.Verify(s => s.Tick(), Times.Once);
        }

        [Test]
        public void StateMachineWithTransition_NoParametersAreSet_DefaultStateIsUsed()
        {
            var defaultStateMock = new Mock<IState>();
            var otherStateMock = new Mock<IState>();
            var stateMachine = new StateMachine(defaultStateMock.Object);
            stateMachine.AddTransition(
                defaultStateMock.Object,
                new Transition(new HashSet<string> { TestParams.ToOtherState }, otherStateMock.Object)
            );

            stateMachine.Tick();

            defaultStateMock.Verify(s => s.Tick(), Times.Once);
            otherStateMock.Verify(s => s.Tick(), Times.Never);
        }

        [Test]
        public void StateMachineWithTransition_ParametersAreSet_OtherStateIsUsed()
        {
            var defaultStateMock = new Mock<IState>();
            var otherStateMock = new Mock<IState>();
            var stateMachine = new StateMachine(defaultStateMock.Object);
            stateMachine.AddTransition(
                defaultStateMock.Object,
                new Transition(new HashSet<string> { TestParams.ToOtherState }, otherStateMock.Object)
            );

            stateMachine.SetTrigger(TestParams.ToOtherState);
            stateMachine.Tick();

            otherStateMock.Verify(s => s.Tick(), Times.Once);
            defaultStateMock.Verify(s => s.Tick(), Times.Never);
        }

        [Test]
        public void GivenMultiplePossibleTransitions_OneWithHighestPriorityIsUsed()
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
            stateMachine.Tick();

            higherPriorityStateMock.Verify(s => s.Tick(), Times.Once);
            lowerPriorityStateMock.Verify(s => s.Tick(), Times.Never);
        }

        [Test]
        public void GivenMultiplePossibleTransitionsWithSamePriority_OneWithMoreParametersIsUsed()
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
            stateMachine.Tick();

            higherPriorityStateMock.Verify(s => s.Tick(), Times.Once);
            lowerPriorityStateMock.Verify(s => s.Tick(), Times.Never);
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
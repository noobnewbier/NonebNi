using System;
using Moq;
using NonebNi.Core.Commands;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Maps;
using NUnit.Framework;

namespace NonebNi.EditModeTests.GameLogic
{
    public class CommandEvaluationTests
    {
        [Test]
        public void Evaluate_TargetHasNoHealth_UnitsAreRemovedFromMap()
        {
            var mockMap = new Mock<IMap>();
            var stubNullCommand = new Mock<ICommand>();
            var deadUnit = TestData.CreateDeadUnit();
            stubNullCommand.Setup(c => c.Evaluate()).Returns(() => new[] { deadUnit });
            mockMap.Setup(m => m.Remove(deadUnit)).Returns(true);
            var evaluationService = new CommandEvaluationService(mockMap.Object);

            using var enumerator = evaluationService.Evaluate(stubNullCommand.Object).GetEnumerator();
            while (enumerator.MoveNext())
            {
            }

            mockMap.Verify(m => m.Remove(deadUnit), Times.Once);
        }

        [Test]
        public void Evaluate_TargetHasNoHealthAndIsNotInMap_ExceptionIsThrown()
        {
            var mockMap = new Mock<IMap>();
            var stubNullCommand = new Mock<ICommand>();
            var deadUnit = TestData.CreateDeadUnit();
            stubNullCommand.Setup(c => c.Evaluate()).Returns(() => new[] { deadUnit });
            mockMap.Setup(m => m.Remove(deadUnit)).Returns(false);
            var evaluationService = new CommandEvaluationService(mockMap.Object);

            using var enumerator = evaluationService.Evaluate(stubNullCommand.Object).GetEnumerator();
            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());

            mockMap.Verify(m => m.Remove(deadUnit), Times.Once);
        }

        [Test]
        public void Evaluate_TargetHasHealth_UnitsAreNotRemovedFromMap()
        {
            var mockMap = new Mock<IMap>();
            var stubNullCommand = new Mock<ICommand>();
            var livingUnit = TestData.CreateLivingUnit();
            stubNullCommand.Setup(c => c.Evaluate()).Returns(() => new[] { livingUnit });
            var evaluationService = new CommandEvaluationService(mockMap.Object);

            using var enumerator = evaluationService.Evaluate(stubNullCommand.Object).GetEnumerator();
            while (enumerator.MoveNext())
            {
            }

            mockMap.Verify(m => m.Remove(livingUnit), Times.Never);
        }
    }
}
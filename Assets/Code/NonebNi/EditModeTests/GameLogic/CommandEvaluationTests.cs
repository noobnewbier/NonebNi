using System;
using JetBrains.Annotations;
using Moq;
using NonebNi.Core.Commands;
using NonebNi.Core.Commands.Handlers;
using NonebNi.Core.Maps;
using NUnit.Framework;

namespace NonebNi.EditModeTests.GameLogic
{
    [UsedImplicitly]
    public class CommandEvaluationTests
    {
        public class DamageCommandHandlerTests
        {
            [Test]
            public void Evaluate_TargetHasNoHealth_UnitsAreRemovedFromMap()
            {
                var mockMap = new Mock<IMap>();
                var deadUnit = TestData.CreateDeadUnit();
                var command = new DamageCommand(0, deadUnit);
                mockMap.Setup(m => m.Remove(deadUnit)).Returns(true);
                var evaluationService = new DamageCommandHandler(mockMap.Object);

                using var enumerator = evaluationService.Evaluate(command).GetEnumerator();
                while (enumerator.MoveNext()) { }

                mockMap.Verify(m => m.Remove(deadUnit), Times.Once);
            }

            [Test]
            public void Evaluate_TargetHasNoHealthAndIsNotInMap_ExceptionIsThrown()
            {
                var mockMap = new Mock<IMap>();
                var deadUnit = TestData.CreateDeadUnit();
                var command = new DamageCommand(0, deadUnit);
                mockMap.Setup(m => m.Remove(deadUnit)).Returns(false);
                var evaluationService = new DamageCommandHandler(mockMap.Object);

                using var enumerator = evaluationService.Evaluate(command).GetEnumerator();
                Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());

                mockMap.Verify(m => m.Remove(deadUnit), Times.Once);
            }

            [Test]
            public void Evaluate_TargetHasHealth_UnitsAreNotRemovedFromMap()
            {
                var mockMap = new Mock<IMap>();
                var livingUnit = TestData.CreateLivingUnit();
                var command = new DamageCommand(0, livingUnit);
                var evaluationService = new DamageCommandHandler(mockMap.Object);

                using var enumerator = evaluationService.Evaluate(command).GetEnumerator();
                while (enumerator.MoveNext()) { }

                mockMap.Verify(m => m.Remove(livingUnit), Times.Never);
            }
        }
    }
}
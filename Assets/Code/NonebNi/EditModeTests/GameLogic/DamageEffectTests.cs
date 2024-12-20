using System;
using Moq;
using NonebNi.Core.Effects;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NUnit.Framework;

namespace NonebNi.EditModeTests.GameLogic
{
    [TestFixture]
    public class DamageEffectTests
    {
        private readonly DamageEffect _damageEffect = new("none", 0);

        [Test]
        public void Evaluate_TargetHasNoHealth_UnitsAreRemovedFromMap()
        {
            var mockMap = new Mock<IMap>();
            var deadUnit = TestData.CreateDeadUnit();
            mockMap.Setup(m => m.Remove(deadUnit)).Returns(true);

            _damageEffect
                .Evaluate(mockMap.Object, SystemEntity.Instance, new[] { deadUnit })
                .EvaluateEnumerable();

            mockMap.Verify(m => m.Remove(deadUnit), Times.Once);
        }

        [Test]
        public void Evaluate_TargetHasNoHealthAndIsNotInMap_ExceptionIsThrown()
        {
            var mockMap = new Mock<IMap>();
            var deadUnit = TestData.CreateDeadUnit();
            mockMap.Setup(m => m.Remove(deadUnit)).Returns(false);

            using var enumerator = _damageEffect
                .Evaluate(mockMap.Object, SystemEntity.Instance, new[] { deadUnit })
                .GetEnumerator();
            Assert.Throws<InvalidOperationException>(() => enumerator.MoveNext());

            mockMap.Verify(m => m.Remove(deadUnit), Times.Once);
        }

        [Test]
        public void Evaluate_TargetHasHealth_UnitsAreNotRemovedFromMap()
        {
            var mockMap = new Mock<IMap>();
            var livingUnit = TestData.CreateLivingUnit();

            _damageEffect
                .Evaluate(mockMap.Object, SystemEntity.Instance, new[] { livingUnit })
                .EvaluateEnumerable();

            mockMap.Verify(m => m.Remove(livingUnit), Times.Never);
        }
    }
}
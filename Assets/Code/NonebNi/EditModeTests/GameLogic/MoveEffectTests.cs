using System.Linq;
using Moq;
using NonebNi.Core.Actions;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Effects;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using NUnit.Framework;

namespace NonebNi.EditModeTests.GameLogic
{
    [TestFixture]
    public class MoveEffectTests
    {
        private readonly MoveEffect _moveEffect = new();
        private readonly Coordinate _targetCoord = new();

        [Test]
        public void Evaluate_TargetIsCoordinate_TryMoveUnitToCoordinate()
        {
            var mockMap = new Mock<IMap>();
            var unit = TestData.CreateLivingUnit();

            _moveEffect
                .Evaluate(mockMap.Object, unit, new IActionTarget[] { _targetCoord })
                .EvaluateEnumerable();

            mockMap.Verify(m => m.Move(unit, _targetCoord), Times.Once);
        }

        [Test]
        public void Evaluate_ActorIsNotUnit_StillTryToMoveActor()
        {
            var mockMap = new Mock<IMap>();
            var wallObject = TestData.CreateWallObject();

            _moveEffect
                .Evaluate(mockMap.Object, wallObject, new IActionTarget[] { _targetCoord })
                .EvaluateEnumerable();

            mockMap.Verify(m => m.Move(wallObject, _targetCoord), Times.Once);
        }

        [Test]
        public void Evaluate_TargetEmpty_ReturnMoveSequence()
        {
            var mockMap = new Mock<IMap>();
            var unit = TestData.CreateLivingUnit();
            mockMap.Setup(m => m.Move(unit, _targetCoord)).Returns(MoveResult.Success);

            var sequences = _moveEffect.Evaluate(
                mockMap.Object,
                unit,
                new IActionTarget[] { _targetCoord }
            ).ToArray();

            Assert.That(sequences.Length, Is.EqualTo(1));
            Assert.That(sequences.FirstOrDefault(), Is.TypeOf<MoveSequence>());
        }

        [Test]
        public void Evaluate_TargetOccupied_NoMoveSequenceReturned()
        {
            var mockMap = new Mock<IMap>();
            var unit = TestData.CreateLivingUnit();
            mockMap.Setup(m => m.Move(unit, _targetCoord)).Returns(MoveResult.ErrorTargetOccupied);

            var sequences = _moveEffect.Evaluate(
                mockMap.Object,
                unit,
                new IActionTarget[] { _targetCoord }
            );

            Assert.That(sequences, Is.Empty);
        }
    }
}
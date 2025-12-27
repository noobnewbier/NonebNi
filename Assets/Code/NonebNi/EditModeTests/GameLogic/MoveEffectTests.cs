using System.Linq;
using Moq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Effects;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Pathfinding;
using NonebNi.Core.Sequences;
using NonebNi.Core.Units;
using NUnit.Framework;

namespace NonebNi.EditModeTests.GameLogic
{
    [TestFixture]
    public class MoveEffectTests
    {
        private readonly MoveEffect _moveEffect = new ();
        private readonly Coordinate _targetCoord = new ();

        [Test]
        public void Evaluate_TargetIsCoordinate_TryMoveUnitToCoordinate()
        {
            var unit = TestData.CreateLivingUnit();
            var mockMap = new Mock<IMap>();
            var pathFindingService = new Mock<IPathfindingService>();
            pathFindingService.Setup(service => service.FindPath(It.IsAny<EntityData>(), It.IsAny<Coordinate>()))
                              .Returns(() => (true, new[] { new Coordinate() }));
            var evaluator = new MoveEffect.Evaluator(pathFindingService.Object);

            evaluator.Evaluate(_moveEffect, mockMap.Object, unit, _targetCoord)
                     .EvaluateEnumerable();

            mockMap.Verify(m => m.Move(unit, _targetCoord), Times.Once);
        }

        [Test]
        public void Evaluate_ActorIsNotUnit_StillTryToMoveActor()
        {
            var wallObject = TestData.CreateWallObject();
            var mockMap = new Mock<IMap>();
            var pathFindingService = new Mock<IPathfindingService>();
            pathFindingService.Setup(service => service.FindPath(It.IsAny<EntityData>(), It.IsAny<Coordinate>()))
                              .Returns(() => (true, Enumerable.Empty<Coordinate>()));
            var evaluator = new MoveEffect.Evaluator(pathFindingService.Object);
            evaluator.Evaluate(_moveEffect, mockMap.Object, wallObject, _targetCoord)
                     .EvaluateEnumerable();

            mockMap.Verify(m => m.Move(wallObject, _targetCoord), Times.Once);
        }

        [Test]
        public void Evaluate_TargetEmpty_ReturnMoveSequence()
        {
            var unit = TestData.CreateLivingUnit();
            var mockMap = new Mock<IMap>();
            mockMap.Setup(m => m.Move(It.IsAny<UnitData>(), It.IsAny<Coordinate>())).Returns(MoveResult.Success);
            var pathFindingService = new Mock<IPathfindingService>();
            pathFindingService.Setup(service => service.FindPath(It.IsAny<EntityData>(), It.IsAny<Coordinate>()))
                              .Returns(() => (true, new[] { new Coordinate() }));

            var evaluator = new MoveEffect.Evaluator(pathFindingService.Object);

            var sequences = evaluator.Evaluate
                                     (
                                         _moveEffect,
                                         mockMap.Object,
                                         unit,
                                         _targetCoord
                                     )
                                     .ToArray();

            Assert.That(sequences.Length, Is.EqualTo(1));
            Assert.That(sequences.FirstOrDefault(), Is.TypeOf<MoveSequence>());
        }

        [Test]
        public void Evaluate_TargetOccupied_NoMoveSequenceReturned()
        {
            var unit = TestData.CreateLivingUnit();
            var mockMap = new Mock<IMap>();
            mockMap.Setup(m => m.Move(unit, _targetCoord)).Returns(MoveResult.ErrorTargetOccupied);
            var pathFindingService = new Mock<IPathfindingService>();
            pathFindingService.Setup(service => service.FindPath(It.IsAny<EntityData>(), It.IsAny<Coordinate>()))
                              .Returns(() => (false, Enumerable.Empty<Coordinate>()));
            var evaluator = new MoveEffect.Evaluator(pathFindingService.Object);

            var sequences = evaluator.Evaluate
            (
                _moveEffect,
                mockMap.Object,
                unit,
                _targetCoord
            );

            Assert.That(sequences, Is.Empty);
        }
    }
}
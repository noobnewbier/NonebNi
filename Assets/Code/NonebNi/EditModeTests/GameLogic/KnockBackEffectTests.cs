using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Moq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Effects;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using NUnit.Framework;
using Unity.Logging;
using Unity.Logging.Sinks;
using UnityEngine;
using UnityEngine.TestTools;
using Logger = Unity.Logging.Logger;

namespace NonebNi.EditModeTests.GameLogic
{
    [TestFixture]
    public class KnockBackEffectTests
    {
        [SetUp]
        public void SetUp()
        {
            _currentLogger = Log.Logger;
            var debugLogger = new LoggerConfig()
                .SyncMode.FullSync()
                .WriteTo.UnityDebugLog()
                .CreateLogger();
            Log.Logger = debugLogger;
        }

        [TearDown]
        public void TearDown()
        {
            Log.Logger = _currentLogger;
        }

        private readonly Regex _any = new(".*");
        private readonly KnockBackEffect _effect = new(2);
        private Logger _currentLogger = null!;

        [Test]
        public void Evaluate_CasterNotOnMap_ErrorLogReceived()
        {
            var mockMap = new Mock<IMap>();
            var caster = TestData.CreateLivingUnit();
            var target = TestData.CreateLivingUnit();
            Coordinate fakeActorCoord = default;
            mockMap.Setup(m => m.TryFind(caster, out fakeActorCoord)).Returns(false);

            var sequences = _effect.Evaluate(mockMap.Object, caster, target);

            LogAssert.Expect(LogType.Error, _any);
            Assert.That(sequences, Is.Empty);
            mockMap.Verify(m => m.Move(target, It.IsAny<Coordinate>()), Times.Never);
        }

        [Test]
        public void Evaluate_TargetIsCoordinate_ErrorLogReceived()
        {
            var mockMap = new Mock<IMap>();
            var caster = TestData.CreateLivingUnit();
            var target = default(Coordinate);
            Coordinate fakeActorCoord = default;
            mockMap.Setup(m => m.TryFind(caster, out fakeActorCoord)).Returns(true);

            var sequences = _effect.Evaluate(mockMap.Object, caster, target);

            LogAssert.Expect(LogType.Error, _any);
            Assert.That(sequences, Is.Empty);
            mockMap.Verify(m => m.Move(It.IsAny<EntityData>(), It.IsAny<Coordinate>()), Times.Never);
        }


        [Test]
        public void Evaluate_TargetSpansOverMultipleTiles_ErrorLogReceived()
        {
            var mockMap = new Mock<IMap>();

            Coordinate fakeActorCoord = default;
            var caster = TestData.CreateLivingUnit();
            mockMap.Setup(m => m.TryFind(caster, out fakeActorCoord)).Returns(true);

            IEnumerable<Coordinate> fakeTargetCoords = new Coordinate[] { default, default };
            var target = TestData.CreateLivingUnit();
            mockMap.Setup(m => m.TryFind(target, out fakeTargetCoords)).Returns(true);

            var sequences = _effect.Evaluate(mockMap.Object, caster, target);

            LogAssert.Expect(LogType.Error, _any);
            Assert.That(sequences, Is.Empty);
            mockMap.Verify(m => m.Move(target, It.IsAny<Coordinate>()), Times.Never);
        }


        [Test]
        public void Evaluate_TargetNotOnMap_ErrorLogReceived()
        {
            var mockMap = new Mock<IMap>();

            Coordinate fakeActorCoord = default;
            var caster = TestData.CreateLivingUnit();
            mockMap.Setup(m => m.TryFind(caster, out fakeActorCoord)).Returns(true);

            IEnumerable<Coordinate> fakeTargetCoords = Array.Empty<Coordinate>();
            var target = TestData.CreateLivingUnit();
            mockMap.Setup(m => m.TryFind(target, out fakeTargetCoords)).Returns(false);

            var sequences = _effect.Evaluate(mockMap.Object, caster, target);

            LogAssert.Expect(LogType.Error, _any);
            Assert.That(sequences, Is.Empty);
        }


        [Test]
        public void Evaluate_EndPositionIsNotOccupied_TargetMovedAccordingly()
        {
            var mockMap = new Mock<IMap>();

            var casterCoords = new Coordinate(0, 0);
            var caster = TestData.CreateLivingUnit();
            mockMap.Setup(m => m.TryFind(caster, out casterCoords)).Returns(true);

            IEnumerable<Coordinate> targetCoords = new[] { new Coordinate(0, 1) };
            var target = TestData.CreateLivingUnit();
            mockMap.Setup(m => m.IsOccupied(It.IsAny<Coordinate>())).Returns(false);
            mockMap.Setup(m => m.IsCoordinateWithinMap(It.IsAny<Coordinate>())).Returns(true);
            mockMap.Setup(m => m.TryFind(target, out targetCoords)).Returns(true);
            mockMap.Setup(m => m.Move(target, It.IsAny<Coordinate>())).Returns(MoveResult.Success);

            var sequences = _effect.Evaluate(mockMap.Object, caster, target).ToArray();

            var expectedEndCoord = new Coordinate(0, 3);
            LogAssert.NoUnexpectedReceived();
            mockMap.Verify(m => m.Move(target, expectedEndCoord), Times.Once);
            Assert.That(sequences.Length, Is.EqualTo(1));
            Assert.That(sequences.First(), Is.TypeOf<KnockBackSequence>());
        }

        [Test]
        public void Evaluate_EndPositionIsOccupied_FindNonOccupiedPositionToMoveTo()
        {
            var mockMap = new Mock<IMap>();

            var casterCoords = new Coordinate(0, 0);
            var caster = TestData.CreateLivingUnit();
            mockMap.Setup(m => m.TryFind(caster, out casterCoords)).Returns(true);

            var blockedCoordinateOnKnockBackEnd = new Coordinate(0, 3);
            mockMap.Setup(m => m.IsOccupied(It.IsAny<Coordinate>())).Returns(false);
            mockMap.Setup(m => m.IsOccupied(blockedCoordinateOnKnockBackEnd)).Returns(true);
            mockMap.Setup(m => m.IsCoordinateWithinMap(It.IsAny<Coordinate>())).Returns(true);

            var target = TestData.CreateLivingUnit();
            IEnumerable<Coordinate> targetCoords = new[] { new Coordinate(0, 1) };
            mockMap.Setup(m => m.TryFind(target, out targetCoords)).Returns(true);

            var sequences = _effect.Evaluate(mockMap.Object, caster, target).ToArray();

            var expectedEndCoord = new Coordinate(0, 2);
            LogAssert.NoUnexpectedReceived();
            mockMap.Verify(m => m.Move(target, expectedEndCoord), Times.Once);
            Assert.That(sequences.Length, Is.EqualTo(1));
            Assert.That(sequences.First(), Is.TypeOf<KnockBackSequence>());
        }
    }
}
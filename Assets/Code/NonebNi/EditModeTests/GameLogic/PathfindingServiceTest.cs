using System.Collections.Generic;
using JetBrains.Annotations;
using Moq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using NonebNi.Core.Pathfinding;
using NonebNi.Core.Tiles;
using NUnit.Framework;

namespace NonebNi.EditModeTests.GameLogic
{
    [UsedImplicitly]
    public class PathfindingServiceTest
    {
        private Mock<IReadOnlyMap> _mockMap = null!;
        private PathfindingService _pathfindingService = null!;

        [SetUp]
        public void SetUp()
        {
            _mockMap = new Mock<IReadOnlyMap>();
            _pathfindingService = new PathfindingService(_mockMap.Object);
        }

        [Test]
        public void FindPath_PathCostMoreThanSpeed_ReturnFailure()
        {
            SetUpMap(
                new[,]
                {
                    { TestData.Road, TestData.Wall, TestData.Road },
                    { TestData.Road, TestData.Road, TestData.Road },
                    { TestData.Road, TestData.Wall, TestData.Road }
                }
            );

            var unit = TestData.CreateLivingUnit().WithSpeed(1);
            var result = _pathfindingService.FindPath(unit, new StorageCoordinate(2, 2).ToAxial());

            Assert.IsFalse(result.isPathExist);
            Assert.That(result.path, Is.Empty);
        }


        [Test]
        public void FindPath_MultiplePathExist_ReturnShorterPath()
        {
            SetUpMap(
                new[,]
                {
                    { TestData.Road, TestData.Wall, TestData.Road },
                    { TestData.Road, TestData.Road, TestData.Road },
                    { TestData.Road, TestData.Wall, TestData.Road }
                }
            );

            var result = _pathfindingService.FindPath(
                new StorageCoordinate(0, 0).ToAxial(),
                new StorageCoordinate(2, 2).ToAxial()
            );

            Assert.IsTrue(result.isPathExist);
            Assert.That(
                result.path,
                Is.EqualTo(
                    new[]
                    {
                        new StorageCoordinate(0, 1).ToAxial(),
                        new StorageCoordinate(1, 1).ToAxial(),
                        new StorageCoordinate(2, 2).ToAxial()
                    }
                )
            );
        }

        [Test]
        public void FindPath_PathExist_ReturnPath()
        {
            SetUpMap(
                new[,]
                {
                    { TestData.Road, TestData.Road, TestData.Road }
                }
            );

            var result = _pathfindingService.FindPath(
                new StorageCoordinate(0, 0).ToAxial(),
                new StorageCoordinate(2, 0).ToAxial()
            );

            Assert.IsTrue(result.isPathExist);
            Assert.That(
                result.path,
                Is.EqualTo(new[] { new StorageCoordinate(1, 0).ToAxial(), new StorageCoordinate(2, 0).ToAxial() })
            );
        }

        [Test]
        public void FindPath_PathDoesNotExist_ReturnFailure()
        {
            SetUpMap(
                new[,]
                {
                    { TestData.Road, TestData.Wall, TestData.Road }
                }
            );

            var result = _pathfindingService.FindPath(
                new StorageCoordinate(0, 0).ToAxial(),
                new StorageCoordinate(2, 0).ToAxial()
            );

            Assert.IsFalse(result.isPathExist);
            Assert.IsEmpty(result.path);
        }

        private void SetUpMap(TileData[,] datas)
        {
            var allCoordinates = new List<Coordinate>();

            var width = datas.GetLength(1);
            var height = datas.GetLength(0);
            for (var z = 0; z < height; z++)
            for (var x = 0; x < width; x++)
            {
                TileData? data = datas[z, x];
                var coord = new StorageCoordinate(x, z).ToAxial();

                allCoordinates.Add(coord);
                _mockMap.Setup(m => m.TryGet(coord, out data)).Returns(true);
            }

            _mockMap.Setup(m => m.GetAllCoordinates()).Returns(allCoordinates);


            TileData? _ = default;
            _mockMap.Setup(m => m.TryGet(It.IsNotIn(allCoordinates.ToArray()), out _)).Returns(false);
        }
    }
}
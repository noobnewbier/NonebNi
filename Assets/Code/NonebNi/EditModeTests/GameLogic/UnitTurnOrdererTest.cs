using Moq;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Maps;
using NUnit.Framework;

namespace NonebNi.EditModeTests.GameLogic
{
    public class UnitTurnOrdererTest
    {
        [Test]
        public void UnitsWithLargerInitiativeGoesFirst()
        {
            var stubMap = new Mock<IReadOnlyMap>();
            var highPriorityUnit = TestData.CreateHighPriorityUnit();
            var lowPriorityUnit = TestData.CreateLowPriorityUnit();
            stubMap.Setup(m => m.GetAllUnits()).Returns(new[] { lowPriorityUnit, highPriorityUnit });
            var unitsOrderer = new UnitTurnOrderer(stubMap.Object);

            Assert.AreSame(unitsOrderer.CurrentUnit, highPriorityUnit);
            Assert.AreSame(unitsOrderer.ToNextUnit(), lowPriorityUnit);
        }

        [Test]
        public void AfterAllUnitsActOnce_TheFirstUnitActsAgain()
        {
            var stubMap = new Mock<IReadOnlyMap>();
            var highPriorityUnit = TestData.CreateHighPriorityUnit();
            var lowPriorityUnit = TestData.CreateLowPriorityUnit();
            stubMap.Setup(m => m.GetAllUnits()).Returns(new[] { lowPriorityUnit, highPriorityUnit });
            var unitsOrderer = new UnitTurnOrderer(stubMap.Object);

            unitsOrderer.ToNextUnit();

            Assert.AreSame(unitsOrderer.ToNextUnit(), highPriorityUnit);
        }

        [Test]
        public void WhenOnlyOneUnit_TheOnlyUnitsAlwaysActs()
        {
            var stubMap = new Mock<IReadOnlyMap>();
            var highPriorityUnit = TestData.CreateHighPriorityUnit();
            stubMap.Setup(m => m.GetAllUnits()).Returns(new[] { highPriorityUnit });
            var unitsOrderer = new UnitTurnOrderer(stubMap.Object);

            Assert.AreSame(unitsOrderer.ToNextUnit(), highPriorityUnit);
            Assert.AreSame(unitsOrderer.ToNextUnit(), highPriorityUnit);
        }
    }
}
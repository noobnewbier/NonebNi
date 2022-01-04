using System;
using Moq;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Maps;
using NonebNi.Core.Units;
using NonebNi.Core.Units.Skills;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace NonebNi.EditModeTests.GameLogic
{
    public class UnitTurnOrdererTest
    {
        private readonly UnitData _highPriorityUnit = new UnitData(
            nameof(_highPriorityUnit),
            1,
            1,
            GetDefaultSprite(),
            Array.Empty<SkillData>(),
            1
        );

        private readonly UnitData _lowPriorityUnit = new UnitData(
            nameof(_lowPriorityUnit),
            1,
            1,
            GetDefaultSprite(),
            Array.Empty<SkillData>(),
            0
        );

        private static Sprite GetDefaultSprite() =>
            AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        [Test]
        public void UnitsWithLargerInitiativeGoesFirst()
        {
            var stubMap = new Mock<IReadOnlyMap>();
            stubMap.Setup(m => m.GetAllUnits()).Returns(new[] { _lowPriorityUnit, _highPriorityUnit });
            var unitsOrderer = new UnitTurnOrderer(stubMap.Object);

            Assert.AreSame(unitsOrderer.ToNextUnit(), _highPriorityUnit);
            Assert.AreSame(unitsOrderer.ToNextUnit(), _lowPriorityUnit);
        }

        [Test]
        public void AfterAllUnitsActOnce_TheFirstUnitActsAgain()
        {
            var stubMap = new Mock<IReadOnlyMap>();
            stubMap.Setup(m => m.GetAllUnits()).Returns(new[] { _lowPriorityUnit, _highPriorityUnit });
            var unitsOrderer = new UnitTurnOrderer(stubMap.Object);

            unitsOrderer.ToNextUnit();
            unitsOrderer.ToNextUnit();

            Assert.AreSame(unitsOrderer.ToNextUnit(), _highPriorityUnit);
        }

        [Test]
        public void WhenOnlyOneUnit_TheOnlyUnitsAlwaysActs()
        {
            var stubMap = new Mock<IReadOnlyMap>();
            stubMap.Setup(m => m.GetAllUnits()).Returns(new[] { _highPriorityUnit });
            var unitsOrderer = new UnitTurnOrderer(stubMap.Object);

            Assert.AreSame(unitsOrderer.ToNextUnit(), _highPriorityUnit);
            Assert.AreSame(unitsOrderer.ToNextUnit(), _highPriorityUnit);
        }
    }
}
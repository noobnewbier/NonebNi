using System;
using NonebNi.Core.Units;
using NonebNi.Core.Units.Skills;
using UnityEditor;
using UnityEngine;

namespace NonebNi.EditModeTests.GameLogic
{
    public static class TestData
    {
        public static UnitData CreateDeadUnit() =>
            new UnitData(
                Guid.NewGuid(),
                "DeadUnit",
                1,
                0,
                GetDefaultSprite(),
                Array.Empty<SkillData>(),
                0
            );

        public static UnitData CreateLivingUnit() =>
            new UnitData(
                Guid.NewGuid(),
                "LivingUnit",
                1,
                1,
                GetDefaultSprite(),
                Array.Empty<SkillData>(),
                0
            );

        public static UnitData CreateHighPriorityUnit() =>
            new UnitData(
                Guid.NewGuid(),
                "HighPriorityUnit",
                1,
                1,
                GetDefaultSprite(),
                Array.Empty<SkillData>(),
                1
            );

        public static UnitData CreateLowPriorityUnit() =>
            new UnitData(
                Guid.NewGuid(),
                "LowPriorityUnit",
                1,
                1,
                GetDefaultSprite(),
                Array.Empty<SkillData>(),
                0
            );

        private static Sprite GetDefaultSprite() =>
            AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
    }
}
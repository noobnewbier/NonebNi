using System;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;
using NonebNi.Core.Units.Skills;
using UnityEditor;
using UnityEngine;

namespace NonebNi.EditModeTests.GameLogic
{
    public static class TestData
    {
        private const string TestFactionId = "TestFaction";

        public static readonly TileData Road = new("Road", 1);
        public static readonly TileData Wall = new("Wall", TileData.ObstacleWeight);

        public static UnitData CreateDeadUnit() =>
            new(
                Guid.NewGuid(),
                "DeadUnit",
                TestFactionId,
                1,
                0,
                GetDefaultSprite(),
                Array.Empty<SkillData>(),
                0,
                5
            );

        public static UnitData CreateLivingUnit() =>
            new(
                Guid.NewGuid(),
                "LivingUnit",
                TestFactionId,
                1,
                1,
                GetDefaultSprite(),
                Array.Empty<SkillData>(),
                0,
                5
            );

        public static UnitData CreateHighPriorityUnit() =>
            new(
                Guid.NewGuid(),
                "HighPriorityUnit",
                TestFactionId,
                1,
                1,
                GetDefaultSprite(),
                Array.Empty<SkillData>(),
                1,
                5
            );

        public static UnitData CreateLowPriorityUnit() =>
            new(
                Guid.NewGuid(),
                "LowPriorityUnit",
                TestFactionId,
                1,
                1,
                GetDefaultSprite(),
                Array.Empty<SkillData>(),
                0,
                5
            );

        private static Sprite GetDefaultSprite() =>
            AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
    }
}
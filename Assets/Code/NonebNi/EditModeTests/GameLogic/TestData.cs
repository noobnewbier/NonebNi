using System;
using NonebNi.Core.Actions;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;
using UnityEditor;
using UnityEngine;

namespace NonebNi.EditModeTests.GameLogic
{
    public static class TestData
    {
        private const string TestFactionId = "TestFaction";

        public static readonly TileData Road = new("Road", 1, false);
        public static readonly TileData Wall = new("Wall", TileData.ObstacleWeight, true);

        public static TileModifierData CreateWallObject() =>
            new(
                "Wall",
                Guid.NewGuid(),
                TestFactionId,
                Wall,
                true
            );

        public static UnitData CreateDeadUnit() =>
            new(
                Guid.NewGuid(),
                Array.Empty<NonebAction>(),
                GetDefaultSprite(),
                "DeadUnit",
                TestFactionId,
                1,
                0,
                0,
                5,
                0,
                0,
                0,
                0,
                0,
                9,
                15,
                1
            );

        public static UnitData CreateLivingUnit() =>
            new(
                Guid.NewGuid(),
                Array.Empty<NonebAction>(),
                GetDefaultSprite(),
                "LivingUnit",
                TestFactionId,
                1,
                1,
                0,
                5,
                0,
                0,
                0,
                0,
                0,
                9,
                15,
                1
            );

        public static UnitData CreateSlowUnit() =>
            new(
                Guid.NewGuid(),
                Array.Empty<NonebAction>(),
                GetDefaultSprite(),
                "SlowUnit",
                TestFactionId,
                1,
                1,
                0,
                1,
                0,
                0,
                0,
                0,
                0,
                9,
                15,
                1
            );

        public static UnitData CreateHighPriorityUnit() =>
            new(
                Guid.NewGuid(),
                Array.Empty<NonebAction>(),
                GetDefaultSprite(),
                "HighPriorityUnit",
                TestFactionId,
                1,
                1,
                1,
                5,
                0,
                0,
                0,
                0,
                0,
                9,
                15,
                1
            );

        public static UnitData CreateLowPriorityUnit() =>
            new(
                Guid.NewGuid(),
                Array.Empty<NonebAction>(),
                GetDefaultSprite(),
                "LowPriorityUnit",
                TestFactionId,
                1,
                1,
                0,
                5,
                0,
                0,
                0,
                0,
                0,
                9,
                15,
                1
            );

        private static Sprite GetDefaultSprite() =>
            AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
    }
}
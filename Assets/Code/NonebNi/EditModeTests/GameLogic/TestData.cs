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
                "DeadUnit",
                TestFactionId,
                1,
                0,
                GetDefaultSprite(),
                Array.Empty<NonebAction>(),
                0,
                5,
                0,
                0,
                0,
                0
            );

        public static UnitData CreateLivingUnit() =>
            new(
                Guid.NewGuid(),
                "LivingUnit",
                TestFactionId,
                1,
                1,
                GetDefaultSprite(),
                Array.Empty<NonebAction>(),
                0,
                5,
                0,
                0,
                0,
                0
            );

        public static UnitData CreateHighPriorityUnit() =>
            new(
                Guid.NewGuid(),
                "HighPriorityUnit",
                TestFactionId,
                1,
                1,
                GetDefaultSprite(),
                Array.Empty<NonebAction>(),
                1,
                5,
                0,
                0,
                0,
                0
            );

        public static UnitData CreateLowPriorityUnit() =>
            new(
                Guid.NewGuid(),
                "LowPriorityUnit",
                TestFactionId,
                1,
                1,
                GetDefaultSprite(),
                Array.Empty<NonebAction>(),
                0,
                5,
                0,
                0,
                0,
                0
            );

        private static Sprite GetDefaultSprite() =>
            AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
    }
}
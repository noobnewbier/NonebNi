using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Noneb.UI.View;
using NonebNi.Core.Actions;
using NonebNi.Core.Agents;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Maps;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;
using NonebNi.Terrain;
using NonebNi.Ui.Cameras;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using UnityEditor;
using UnityEngine;
using UnityUtils;

namespace NonebNi.Develop
{
    //TODO: fix unit order panel
    //TODO: fix default not active.
    //TODO: some canvas group/animation disable isn't working but we can move on without it - another one for the backlog 
    public class PlayerTurnMenuTestScript : MonoBehaviour
    {
        [SerializeField] private PlayerTurnMenu menu = null!;
        [SerializeField] private NonebViewBehaviour view = null!;
        [SerializeField] private GameObject stackRoot = null!;
        [SerializeField] private TerrainConfigData terrainConfigData = null!;

        private readonly Lazy<UnitData> _unitA = new(() => CreateUnit("A"));
        private readonly Lazy<UnitData> _unitB = new(() => CreateUnit("B"));
        private readonly Lazy<UnitData> _unitC = new(() => CreateUnit("C"));
        private MockInputControl _control = null!;
        private UIStack _stack = null!;

        // ReSharper disable once Unity.IncorrectMethodSignature
        [UsedImplicitly]
        private async UniTaskVoid Start()
        {
            var map = new MockMap(
                new Dictionary<Coordinate, EntityData>
                {
                    [(3, 4)] = _unitA.Value,
                    [(1, 0)] = _unitB.Value,
                    [(9, 8)] = _unitB.Value
                },
                10,
                10
            );
            var orderer = new FakeUniTurnOrderer(_unitA.Value, _unitB.Value, _unitC.Value);
            var playerAgent = new PlayerAgent(TestScriptHelpers.CreateFaction("fake-player"));
            var presenter = new PlayerTurnPresenter(menu, orderer, new CoordinateAndPositionService(terrainConfigData), map, playerAgent);
            _control = new MockInputControl();
            var cameraController = new MockCameraController();
            menu.Init(presenter, _control, cameraController);

            _stack = new UIStack(stackRoot);
            await _stack.Push(view);
        }

        private void OnGUI()
        {
            var rect = new Rect(Screen.width - 170, 10, 150, 25);

            GUI.Label(rect, _control.mode);
            rect.y += 25;
        }

        private static UnitData CreateUnit(string unitName) =>
            new(
                Guid.NewGuid(),
                new[] { ActionDatas.Bash, ActionDatas.Lure, ActionDatas.Shoot },
                AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd"),
                unitName,
                "fake-faction",
                100,
                40,
                10,
                5,
                10,
                10,
                10,
                3,
                10,
                50
            );

        private async UniTask ShowPattern(params UnitData[] units)
        {
            // await widget.Show(units);
        }

        private class FakeUniTurnOrderer : IUnitTurnOrderer
        {
            private readonly CircularBuffer<UnitData> _buffer;

            public FakeUniTurnOrderer(params UnitData[] unitsInOrder)
            {
                _buffer = new CircularBuffer<UnitData>(unitsInOrder);
            }

            public UnitData CurrentUnit => _buffer.Current;

            public IEnumerable<UnitData> UnitsInOrder => _buffer;
            public UnitData ToNextUnit() => _buffer.MoveNext();
        }

        private class MockInputControl : IPlayerTurnWorldSpaceInputControl
        {
            public string mode;
            public Coordinate? FindHoveredCoordinate() => null;

            public void ToMovementMode(UnitData mover)
            {
                mode = "movement";
            }

            public UniTask<IEnumerable<Coordinate>> GetInputForAction(UnitData caster, NonebAction action, CancellationToken token = default)
            {
                mode = "target-selection";
                return new UniTask<IEnumerable<Coordinate>>(Enumerable.Empty<Coordinate>());
            }

            public void ToTileInspectionMode()
            {
                mode = "tile-inspection";
            }

            public void UpdateTargetSelection() { }
        }

        private class MockCameraController : ICameraController
        {
            public void LookAt(Vector3 position) { }

            public void UpdateCamera() { }
        }

        private class MockMap : IReadOnlyMap
        {
            private readonly Dictionary<Coordinate, EntityData> _fakeMap;
            private readonly Dictionary<EntityData, Coordinate> _fakeReversedMap;
            private readonly int _height;
            private readonly int _width;

            public MockMap(Dictionary<Coordinate, EntityData> fakeMap, int height, int width)
            {
                _fakeMap = fakeMap;
                _height = height;
                _width = width;
                _fakeReversedMap = new Dictionary<EntityData, Coordinate>();
                foreach (var (key, value) in fakeMap) _fakeReversedMap[value] = key;
            }

            public bool IsOccupied(Coordinate axialCoordinate) => true;

            public bool TryGet(Coordinate axialCoordinate, [NotNullWhen(true)] out TileData? tileData)
            {
                tileData = TileData.Default;
                return true;
            }

            public TileData Get(Coordinate axialCoordinate) => TileData.Default;

            public T? Get<T>(Coordinate axialCoordinate) where T : EntityData => _fakeMap.GetValueOrDefault(axialCoordinate) as T;

            public bool TryGet<T>(Coordinate axialCoordinate, [NotNullWhen(true)] out T? t) where T : EntityData
            {
                t = Get<T>(axialCoordinate);
                return t != null;
            }

            public bool TryGet(Coordinate axialCoordinate, [NotNullWhen(true)] out IEnumerable<EntityData>? datas)
            {
                var t = Get<EntityData>(axialCoordinate);
                if (t != null)
                {
                    datas = new[] { t };
                    return true;
                }

                datas = null;
                return false;
            }

            public bool Has<T>(Coordinate axialCoordinate) where T : EntityData => Get<T>(axialCoordinate) != null;

            public bool TryFind(EntityData entityData, out Coordinate coordinate) => _fakeReversedMap.TryGetValue(entityData, out coordinate);

            public bool TryFind(EntityData entityData, out IEnumerable<Coordinate> coordinates)
            {
                if (_fakeReversedMap.TryGetValue(entityData, out var coordinate))
                {
                    coordinates = new[] { coordinate };
                    return true;
                }

                coordinates = ArraySegment<Coordinate>.Empty;
                return false;
            }

            public IEnumerable<Coordinate> GetAllCoordinates() => _fakeReversedMap.Values;

            public bool IsCoordinateWithinMap(Coordinate coordinate) => coordinate.X < _width && coordinate.Y < _height && coordinate is { X: >= 0, Y: >= 0 };

            public IEnumerable<UnitData> GetAllUnits() => _fakeReversedMap.Keys.OfType<UnitData>();
            public Coordinate Find(EntityData entityData) => Coordinate.Zero;
        }
    }
}
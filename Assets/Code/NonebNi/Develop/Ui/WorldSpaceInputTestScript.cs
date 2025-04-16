using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.InputSystems;
using NonebNi.Core.Actions;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using NonebNi.Core.Pathfinding;
using NonebNi.Core.Units;
using NonebNi.Terrain;
using NonebNi.Ui.Grids;
using NonebNi.Ui.ViewComponents.PlayerTurn;
using UnityEngine;
using UnityUtils;

namespace NonebNi.Develop
{
    public class WorldSpaceInputTestScript : MonoBehaviour
    {
        [SerializeField] private TerrainConfigData terrainConfig = null!;
        [SerializeField] private Camera viewCamera = null!;
        [SerializeField] private HexHighlightConfig highlightConfig = null!;
        [SerializeField] private GameObject fakeUnitObj = null!;
        [SerializeField] private GameObject fakeEnemyObj = null!;
        private readonly Plane _plane = new(Vector3.up, Vector3.zero);


        private CircularBuffer<NonebAction> _actionBuffer = null!;
        private PlayerTurnWorldSpaceInputControl _control = null!;
        private CoordinateAndPositionService _coordService = null!;

        private CancellationTokenSource? _cts;
        private HexHighlighter? _highlighter;
        private bool _isInitialised;
        private Map _map = null!;

        private Coordinate[]? _testInputs;
        private UnitData _unitData = null!;

        private void Awake()
        {
            _isInitialised = true;
            var inputSystem = new NonebInputSystem();
            _coordService = new CoordinateAndPositionService(terrainConfig);
            _map = new Map(20, 20);
            _highlighter = new HexHighlighter(_coordService, highlightConfig, terrainConfig);
            _actionBuffer = new CircularBuffer<NonebAction>(ActionDatas.Lure, ActionDatas.Shoot, ActionDatas.Bash);
            var targetFinder = new TargetFinder(_map);
            var pathFindingService = new PathfindingService(_map);

            _unitData = TestScriptHelpers.CreateUnit("TestPlayer", "player");
            var unitCoord = _coordService.NearestCoordinateForPoint(fakeUnitObj.transform.position);
            _map.Put(unitCoord, _unitData);

            var enemy = TestScriptHelpers.CreateUnit("TestEnemy", "enemy");
            var enemyCoord = _coordService.NearestCoordinateForPoint(fakeEnemyObj.transform.position);
            _map.Put(enemyCoord, enemy);

            _control = new PlayerTurnWorldSpaceInputControl(inputSystem, _coordService, terrainConfig, viewCamera, _map, _highlighter, targetFinder, pathFindingService);
        }

        private void OnGUI()
        {
            var startingRect = new Rect(10, 10, 150, 25);
            var rect = startingRect;
            if (!_isInitialised)
            {
                GUI.Label(rect, "Start the scene to test");
                return;
            }

            if (GUI.Button(rect, "Use TileInspection")) TestHighlightFlow();
            rect.y += 25;

            if (GUI.Button(rect, "Use Movement")) TestMovementFlow();
            rect.y += 25;

            if (GUI.Button(rect, "Use TargetSelection")) TestInputFlow();
            rect.y += 25;

            if (GUI.Button(rect, $"Cycle Action({_actionBuffer.Current.Name.GetLocalized()})")) _actionBuffer.MoveNext();
            rect.y += 25;

            var (success, pos) = TestScriptHelpers.FindMousePosInWorld(_plane);
            if (success)
            {
                var hoveredCoord = _coordService.NearestCoordinateForPoint(pos);
                GUI.Label(rect, $"Hover input: {hoveredCoord}");
                rect.y += 25;
            }

            if (_testInputs is not null)
            {
                GUI.Label(rect, "Player input:");
                rect.y += 25;

                foreach (var input in _testInputs)
                {
                    GUI.Label(rect, $"{input}");
                    rect.y += 25;
                }
            }

            GUI.Box(new Rect(startingRect.x, startingRect.y, startingRect.width, rect.y - startingRect.y), string.Empty);
        }

        private void OnDrawGizmos()
        {
            if (!_isInitialised) return;

            TestScriptHelpers.DrawGridUsingGizmos(_coordService, _map);
        }

        private void TestHighlightFlow()
        {
            _testInputs = null;
            _control.ToTileInspectionMode();
        }

        private void TestMovementFlow()
        {
            TestActionFlow(ActionDatas.Move);
        }

        private void TestInputFlow()
        {
            TestActionFlow(_actionBuffer.Current);
        }

        private void TestActionFlow(NonebAction action)
        {
            async UniTask Do(CancellationToken ct)
            {
                _testInputs = null;

                var inputs = await _control.GetInputForAction(_unitData, action, ct);
                _testInputs = inputs.ToArray();
            }

            _cts?.Cancel();
            /*
             * Yes - UniTask runs one frame after exit playmode.
             * Either change UniTask's PlayerLoopHelper.InsertRunner and get rid of the "run one frame afterward" behaviour or suck it up and deal with it.
             *
             * If I got annoyed enough, I will do something about it, but for now it's a problem for another day.
             *
             * https://github.com/Cysharp/UniTask/issues/543
             * https://github.com/Cysharp/UniTask/blob/8042b29ff87dd5506d7aad72bd6d8d7405985f27/src/UniTask/Assets/Plugins/UniTask/Runtime/PlayerLoopHelper.cs#L204
             */
            _cts = CancellationTokenSource.CreateLinkedTokenSource(Application.exitCancellationToken);

            Do(_cts.Token).Forget();
        }
    }
}
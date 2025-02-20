using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.InputSystems;
using NonebNi.Core.Actions;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using NonebNi.Core.Units;
using NonebNi.Terrain;
using NonebNi.Ui.Grids;
using UnityEngine;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    //TODO: we need to make this testable, it is taking too long without testables and it's not an efficient way to code.
    public interface IPlayerTurnWorldSpaceInputControl
    {
        Coordinate? FindHoveredCoordinate();
        UniTask ToTileInspectionMode();
        UniTask<IEnumerable<Coordinate>> GetInputForAction(UnitData caster, NonebAction action, CancellationToken token = default);
        void UpdateTargetSelection();
    }

    public class PlayerTurnWorldSpaceInputControl : IPlayerTurnWorldSpaceInputControl
    {
        //TODO: we need a way to make these a bit more reliable, these are jank... at best even frog is doing better.
        private const string NormalHighlightId = "normal";
        private const string TargetHighlightId = "target";
        private const string AreaHintId = "area-hint";

        private readonly ICoordinateAndPositionService _coordinateAndPositionService;
        private readonly Plane _gridPlane;
        private readonly IHexHighlighter _hexHighlighter;
        private readonly IInputSystem _inputSystem;
        private readonly Map _map;
        private readonly Camera _playerViewCamera;
        private readonly ITargetFinder _targetFinder;

        private CancellationTokenSource? _cts;


        public PlayerTurnWorldSpaceInputControl(
            IInputSystem inputSystem,
            ICoordinateAndPositionService coordinateAndPositionService,
            TerrainConfigData terrainConfigData,
            Camera playerViewCamera,
            Map map,
            IHexHighlighter hexHighlighter,
            ITargetFinder targetFinder)
        {
            _inputSystem = inputSystem;
            _coordinateAndPositionService = coordinateAndPositionService;
            _playerViewCamera = playerViewCamera;
            _map = map;
            _hexHighlighter = hexHighlighter;
            _targetFinder = targetFinder;
            _gridPlane = terrainConfigData.GridPlane;
        }

        public Coordinate? FindHoveredCoordinate()
        {
            var point = _inputSystem.MousePosition;
            var ray = _playerViewCamera.ScreenPointToRay(point);
            if (!_gridPlane.Raycast(ray, out var distance)) return null;

            var coord = _coordinateAndPositionService.NearestCoordinateForPoint(ray.GetPoint(distance));
            if (!_map.IsCoordinateWithinMap(coord)) return null;

            return coord;
        }

        public async UniTask<IEnumerable<Coordinate>> GetInputForAction(UnitData caster, NonebAction action, CancellationToken token = default)
        {
            async UniTask<Coordinate?> GetUserInputForRequest((RangeStatus status, Coordinate coord)[] ranges, CancellationToken ct)
            {
                Coordinate? inputCoord = null;
                while (!ct.IsCancellationRequested && inputCoord == null)
                {
                    await UniTask.NextFrame();

                    _hexHighlighter.RemoveRequest(HighlightRequestId.TargetSelection);
                    var coord = FindHoveredCoordinate();
                    if (coord == null) continue;

                    // Keep showing the highlight - we are good
                    _hexHighlighter.RequestHighlight(coord, HighlightRequestId.TargetSelection, TargetHighlightId);

                    if (!_inputSystem.LeftClick) continue;


                    (RangeStatus status, Coordinate _)? matchingRangeStatus = ranges.FirstOrDefault(t => t.coord == coord);
                    if (matchingRangeStatus is not { status: RangeStatus.Targetable })
                        //todo: signal invalid input - potentially audio and even a tooltip to explain why shit is wrong
                        continue;

                    //todo: highlight color need to change depending on if the selection is valid.
                    inputCoord = coord;
                }

                _hexHighlighter.RemoveRequest(HighlightRequestId.AreaHint);
                return inputCoord;
            }

            async UniTask<IEnumerable<Coordinate>> Do(CancellationToken ct)
            {
                _hexHighlighter.ClearAll();

                var playerInputs = new List<Coordinate>();
                foreach (var request in action.TargetRequests)
                {
                    ct.ThrowIfCancellationRequested();

                    var ranges = _targetFinder.FindRange(caster, request).ToArray();
                    _hexHighlighter.RemoveRequest(HighlightRequestId.AreaHint);
                    _hexHighlighter.RequestHighlight(ranges.Select(r => r.coord), HighlightRequestId.AreaHint, AreaHintId);

                    var input = await GetUserInputForRequest(ranges, ct);
                    if (input != null) playerInputs.Add(input);
                }

                _hexHighlighter.RemoveRequest(HighlightRequestId.TargetSelection, HighlightRequestId.AreaHint);

                return playerInputs;
            }

            _cts?.Cancel();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            var coordinates = await Do(_cts.Token);

            return coordinates;
        }

        public async UniTask ToTileInspectionMode()
        {
            async UniTask RunUpdateLoopUntilCancelled(CancellationToken ct)
            {
                while (!ct.IsCancellationRequested)
                {
                    _hexHighlighter.RemoveHighlight(TileInspectionHighlightId);

                    var coord = FindHoveredCoordinate();
                    if (coord.HasValue) _hexHighlighter.AddHighlight(coord.Value, TileInspectionHighlightId);

                    await UniTask.NextFrame();
                }
            }

            _cts?.Cancel();
            await UniTask.WaitUntil(() => _updateLoopTask?.Task.Status != UniTaskStatus.Pending);

            _cts = new CancellationTokenSource();
            await RunUpdateLoopUntilCancelled(_cts.Token);
            _hexHighlighter.RemoveHighlight(TileInspectionHighlightId);
        }

        public void UpdateTargetSelection()
        {
            _hexHighlighter.RemoveHighlight(TargetSelectionHighlightId);

            var coord = FindHoveredCoordinate();
            if (!coord.HasValue) return;

            _hexHighlighter.AddHighlight(coord.Value, TargetSelectionHighlightId);
        }
    }
}
using System.Collections.Generic;
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
using UnityEngine;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    //TODO: we need to make this testable, it is taking too long without testables and it's not an efficient way to code.
    public interface IPlayerTurnWorldSpaceInputControl
    {
        Coordinate? FindHoveredCoordinate();
        void ToTileInspectionMode();
        UniTask<IEnumerable<Coordinate>> GetInputForAction(UnitData caster, NonebAction action, CancellationToken token = default);
        void UpdateTargetSelection();
    }

    public class PlayerTurnWorldSpaceInputControl : IPlayerTurnWorldSpaceInputControl
    {
        private readonly ICoordinateAndPositionService _coordinateAndPositionService;

        private readonly Plane _gridPlane;
        private readonly IHexHighlighter _hexHighlighter;
        private readonly IInputSystem _inputSystem;
        private readonly IReadOnlyMap _map;
        private readonly IPathfindingService _pathfindingService;
        private readonly Camera _playerViewCamera;
        private readonly ITargetFinder _targetFinder;

        private CancellationTokenSource? _cts;


        public PlayerTurnWorldSpaceInputControl(
            IInputSystem inputSystem,
            ICoordinateAndPositionService coordinateAndPositionService,
            TerrainConfigData terrainConfigData,
            Camera playerViewCamera,
            IReadOnlyMap map,
            IHexHighlighter hexHighlighter,
            ITargetFinder targetFinder,
            IPathfindingService pathfindingService)
        {
            _inputSystem = inputSystem;
            _coordinateAndPositionService = coordinateAndPositionService;
            _playerViewCamera = playerViewCamera;
            _map = map;
            _hexHighlighter = hexHighlighter;
            _targetFinder = targetFinder;
            _pathfindingService = pathfindingService;
            _gridPlane = terrainConfigData.GridPlane;
        }

        public Coordinate? FindHoveredCoordinate()
        {
            //TODO: it probably makes sense to put this input code into the input system - but then it's in game ony...
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
                    (RangeStatus status, Coordinate _)? matchingRangeStatus = ranges.FirstOrDefault(t => t.coord == coord);
                    var isValidInput = matchingRangeStatus is { status: RangeStatus.Targetable };
                    var variation = isValidInput ?
                        HighlightVariation.ValidInput :
                        HighlightVariation.InvalidInput;
                    _hexHighlighter.RequestHighlight(coord, HighlightRequestId.TargetSelection, variation);

                    if (!_inputSystem.LeftClick) continue;

                    if (!isValidInput)
                        //todo: signal invalid input - potentially audio and even a tooltip to explain why shit is wrong
                        continue;

                    //todo: highlight color need to change depending on if the selection is valid.
                    inputCoord = coord;
                }

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
                    _hexHighlighter.RequestHighlight(ranges.Select(r => r.coord), HighlightRequestId.AreaHint, HighlightVariation.AreaHint);

                    var input = await GetUserInputForRequest(ranges, ct);
                    _hexHighlighter.RemoveRequest(HighlightRequestId.AreaHint);

                    if (input != null) playerInputs.Add(input);
                }

                _hexHighlighter.RemoveRequest(HighlightRequestId.TargetSelection, HighlightRequestId.AreaHint);

                return playerInputs;
            }


            if (action == ActionDatas.Move)
            {
                var inputForMovement = await GetInputForMovement(caster, token);
                if (inputForMovement != null) return new[] { inputForMovement };

                return Enumerable.Empty<Coordinate>();
            }

            _cts?.Cancel();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            var coordinates = await Do(_cts.Token);

            return coordinates;
        }

        public void ToTileInspectionMode()
        {
            async UniTaskVoid Do(CancellationToken ct)
            {
                _hexHighlighter.ClearAll();
                while (!ct.IsCancellationRequested)
                {
                    _hexHighlighter.RemoveRequest(HighlightRequestId.TileInspection);

                    var coord = FindHoveredCoordinate();
                    if (coord != null) _hexHighlighter.RequestHighlight(coord, HighlightRequestId.TileInspection, HighlightVariation.Normal);

                    await UniTask.NextFrame();
                }

                _hexHighlighter.RemoveRequest(HighlightRequestId.TileInspection);
            }

            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            Do(_cts.Token).Forget();
        }

        //TODO: what is this for...?
        public void UpdateTargetSelection()
        {
            _hexHighlighter.RemoveRequest(HighlightRequestId.TargetSelection);

            var coord = FindHoveredCoordinate();
            if (coord == null) return;

            _hexHighlighter.RequestHighlight(coord, HighlightRequestId.TargetSelection, HighlightVariation.Normal);
        }
    }
}
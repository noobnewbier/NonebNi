using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.InputSystems;
using NonebNi.Core.Actions;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Decisions;
using NonebNi.Core.Entities;
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
        UniTask<IEnumerable<Coordinate>> GetInputForAction(UnitData caster, NonebAction action, CancellationToken ct = default);
        UniTask<EntityData?> GetInputForInspection(CancellationToken ct = default);
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
        private readonly IDecisionValidator _validator;

        private CancellationTokenSource? _cts;


        public PlayerTurnWorldSpaceInputControl(
            IInputSystem inputSystem,
            ICoordinateAndPositionService coordinateAndPositionService,
            TerrainConfigData terrainConfigData,
            Camera playerViewCamera,
            IReadOnlyMap map,
            IHexHighlighter hexHighlighter,
            ITargetFinder targetFinder,
            IPathfindingService pathfindingService,
            IDecisionValidator validator)
        {
            _inputSystem = inputSystem;
            _coordinateAndPositionService = coordinateAndPositionService;
            _playerViewCamera = playerViewCamera;
            _map = map;
            _hexHighlighter = hexHighlighter;
            _targetFinder = targetFinder;
            _pathfindingService = pathfindingService;
            _validator = validator;
            _gridPlane = terrainConfigData.GridPlane;
        }

        public Coordinate? FindHoveredCoordinate()
        {
            if (_inputSystem.IsMouseOverUi) return null;

            //TODO: it probably makes sense to put this input code into the input system - but then it's in game ony...
            var point = _inputSystem.MousePosition;
            var ray = _playerViewCamera.ScreenPointToRay(point);
            if (!_gridPlane.Raycast(ray, out var distance)) return null;

            var coord = _coordinateAndPositionService.NearestCoordinateForPoint(ray.GetPoint(distance));
            if (!_map.IsCoordinateWithinMap(coord)) return null;

            return coord;
        }

        public async UniTask<IEnumerable<Coordinate>> GetInputForAction(UnitData caster, NonebAction action, CancellationToken ct = default)
        {
            async UniTask<Coordinate?> GetUserInputForRequest(IReadOnlyList<Coordinate> inputForPreviousRequests, CancellationToken subCt)
            {
                Coordinate? inputCoord = null;
                while (inputCoord == null)
                {
                    await UniTask.NextFrame(subCt, true);

                    _hexHighlighter.RemoveRequest(HighlightRequestId.TargetSelection);
                    var coord = FindHoveredCoordinate();
                    if (coord == null) continue;

                    // Keep showing the highlight - we are good
                    var (canBeValid, _) = _validator.ValidateDecisionConstructionInput(action, caster, inputForPreviousRequests, coord);
                    var variation = canBeValid ?
                        HighlightVariation.ValidInput :
                        HighlightVariation.InvalidInput;
                    _hexHighlighter.RequestHighlight(coord, HighlightRequestId.TargetSelection, variation);

                    if (!_inputSystem.LeftClick) continue;

                    if (!canBeValid)
                        //todo: signal invalid input - potentially audio and even a tooltip to explain why shit is wrong
                        continue;

                    //todo: highlight color need to change depending on if the selection is valid.
                    inputCoord = coord;
                }

                return inputCoord;
            }

            async UniTask<IEnumerable<Coordinate>> Do(CancellationToken subCt)
            {
                try
                {
                    var playerInputs = new List<Coordinate>();
                    foreach (var request in action.TargetRequests)
                    {
                        var ranges = _targetFinder.FindRange(caster, request).ToArray();

                        _hexHighlighter.RemoveRequest(HighlightRequestId.AreaHint);
                        _hexHighlighter.RequestHighlight(ranges.Select(r => r.coord), HighlightRequestId.AreaHint, HighlightVariation.AreaHint);

                        var input = await GetUserInputForRequest(playerInputs, subCt);
                        subCt.ThrowIfCancellationRequested();

                        _hexHighlighter.RemoveRequest(HighlightRequestId.AreaHint);

                        if (input != null) playerInputs.Add(input);
                    }

                    return playerInputs;
                }
                finally
                {
                    _hexHighlighter.RemoveRequest(HighlightRequestId.TargetSelection, HighlightRequestId.AreaHint);
                }
            }


            if (action == ActionDatas.Move)
            {
                var inputForMovement = await GetInputForMovement(caster, ct);
                if (inputForMovement != null) return new[] { inputForMovement };

                return Enumerable.Empty<Coordinate>();
            }

            //todo: we should, really, really wait till the cancellation is done before starting the next one.
            _cts?.Cancel();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var coordinates = await Do(_cts.Token);

            return coordinates;
        }

        public void ToTileInspectionMode()
        {
            async UniTaskVoid Do(CancellationToken ct)
            {
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

        //todo: v.close to combo being executed... keep at it
        public async UniTask<EntityData?> GetInputForInspection(CancellationToken ct = default)
        {
            async UniTask<EntityData?> GetUserInputForRequest(CancellationToken subCt)
            {
                try
                {
                    EntityData? inputEntity = null;
                    while (inputEntity == null)
                    {
                        await UniTask.NextFrame(subCt, true);

                        _hexHighlighter.RemoveRequest(HighlightRequestId.TileInspection);
                        var coord = FindHoveredCoordinate();
                        if (coord == null) continue;

                        _hexHighlighter.RequestHighlight(coord, HighlightRequestId.TileInspection, HighlightVariation.Normal);

                        if (!_inputSystem.LeftClick) continue;

                        inputEntity = _map.Get<EntityData>(coord);
                    }

                    return inputEntity;
                }
                finally
                {
                    _hexHighlighter.RemoveRequest(HighlightRequestId.TileInspection);
                }
            }

            _cts?.Cancel();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var entity = await GetUserInputForRequest(_cts.Token);

            return entity;
        }

        private async UniTask<Coordinate?> GetInputForMovement(UnitData mover, CancellationToken token = default)
        {
            /*
             * In theory we can use GetInputForAction, just that the UI is slightly different so we want to have a variant anyway
             */

            async UniTask<Coordinate?> GetUserInputForRequest(CancellationToken ct)
            {
                try
                {
                    Coordinate? inputCoord = null;
                    while (!ct.IsCancellationRequested && inputCoord == null)
                    {
                        await UniTask.NextFrame();

                        _hexHighlighter.RemoveRequest(HighlightRequestId.MovementHint);
                        var coord = FindHoveredCoordinate();
                        if (coord == null) continue;

                        var (isPathExist, path) = _pathfindingService.FindPath(mover, coord);
                        if (!isPathExist)
                        {
                            _hexHighlighter.RequestHighlight(coord, HighlightRequestId.MovementHint, HighlightVariation.InvalidInput);
                            continue;
                        }

                        var pathWithoutStartAndEnd = path.Except(new[] { _map.Find(mover), coord });
                        _hexHighlighter.RequestHighlight(pathWithoutStartAndEnd, HighlightRequestId.MovementHint, HighlightVariation.AreaHint);
                        _hexHighlighter.RequestHighlight(coord, HighlightRequestId.MovementHint, HighlightVariation.Normal);

                        if (!_inputSystem.LeftClick) continue;
                        inputCoord = coord;
                    }

                    return inputCoord;
                }
                finally
                {
                    _hexHighlighter.RemoveRequest(HighlightRequestId.MovementHint);
                }
            }

            _cts?.Cancel();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            var coordinate = await GetUserInputForRequest(_cts.Token);

            return coordinate;
        }
    }
}
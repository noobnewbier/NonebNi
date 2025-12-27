using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.Logs.Runtime;
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
using NonebNi.Ui.Inputs;
using UnityEngine;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    //TODO: we need to make this testable, it is taking too long without testables and it's not an efficient way to code.
    public interface IPlayerTurnWorldSpaceInputControl
    {
        Coordinate? FindHoveredCoordinate();

        UniTask<(bool success, IEnumerable<Coordinate>)> GetInputForAction(UnitData caster, NonebAction action, CancellationToken ct = default);

        /// <summary>
        /// This only works with unit atm only because the UI doesn't really have much to show for anything else, this can change
        /// in the future though.
        /// </summary>
        UniTask<UnitData?> GetInputForInspection(CancellationToken ct = default);

        UniTask<MovementInput> GetInputForMovement(UnitData mover, CancellationToken ct = default);

        public abstract record MovementInput
        {
            public record Inspect : MovementInput
            {
                public readonly EntityData EntityData;

                public Inspect(EntityData entityData)
                {
                    EntityData = entityData;
                }
            }

            public record MoveTo : MovementInput
            {
                public readonly Coordinate Coordinate;

                public MoveTo(Coordinate coordinate)
                {
                    Coordinate = coordinate;
                }
            }

            public record Cancel : MovementInput;
        }
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
            var point = _inputSystem.ReadValue<Vector2>(InputMaps.UI.Point);
            var ray = _playerViewCamera.ScreenPointToRay(point);
            if (!_gridPlane.Raycast(ray, out var distance)) return null;

            var coord = _coordinateAndPositionService.NearestCoordinateForPoint(ray.GetPoint(distance));
            if (!_map.IsCoordinateWithinMap(coord)) return null;

            return coord;
        }

        //todo: show invalid tooltip - it helps with debugging as well.
        public async UniTask<(bool success, IEnumerable<Coordinate>)> GetInputForAction(UnitData caster, NonebAction action, CancellationToken ct = default)
        {
            async UniTask<(bool backRequest, Coordinate? input)> GetUserInputForRequest(IReadOnlyList<Coordinate> inputForPreviousRequests, TargetRequest currentRequest, CancellationToken subCt)
            {
                Coordinate? inputCoord = null;
                while (inputCoord == null)
                {
                    await UniTask.NextFrame(subCt, true);

                    // remove highlight first - don't want that to stick around
                    _hexHighlighter.RemoveRequest(HighlightRequestId.TargetSelection);

                    // doesn't matter if we are hovering - if we get a back request we just back out
                    if (_inputSystem.GetAction(InputMaps.Level.Cancel)) return (true, null);

                    var coord = FindHoveredCoordinate();
                    if (coord == null) continue;

                    // Keep showing the highlight - we are good
                    var (canBeValid, error) = _validator.ValidateDecisionConstructionInput(action, caster, inputForPreviousRequests, coord);
                    if (!canBeValid && error?.Type == IDecisionValidator.ErrorType.OutOfRange)
                    {
                        _hexHighlighter.RequestHighlight(coord, HighlightRequestId.TargetSelection, HighlightVariation.InvalidInput);
                        continue;
                    }


                    //todo: more sophisticated logic is needed here - the variation might need to change depending if we are hitting enemy/allies
                    var targetedStatuses = _targetFinder.GetTargetedCoordinates(caster, coord, currentRequest).ToArray();
                    var isAnyDangerous = targetedStatuses.Any(s => s.isDangerous);
                    var variation = canBeValid switch
                    {
                        true when isAnyDangerous => HighlightVariation.DangerousInput,
                        true => HighlightVariation.ValidInput,
                        _ => HighlightVariation.InvalidInput
                    };

                    foreach (var (_, coordinate) in targetedStatuses) _hexHighlighter.RequestHighlight(coordinate, HighlightRequestId.TargetSelection, variation);


                    if (!_inputSystem.GetAction(InputMaps.Level.Interact)) continue;

                    if (!canBeValid)
                        //todo: signal invalid input - potentially audio and even a tooltip to explain why shit is wrong
                        continue;

                    //todo: highlight color need to change depending on if the selection is valid.
                    inputCoord = coord;
                }

                return (false, inputCoord);
            }

            async UniTask<(bool, IEnumerable<Coordinate>)> Do(CancellationToken subCt)
            {
                try
                {
                    var playerInputs = new Queue<Coordinate>();
                    for (var i = 0; i < action.TargetRequests.Length; i++)
                    {
                        var request = action.TargetRequests[i];
                        var ranges = _targetFinder.FindRange(caster, request).ToArray();

                        _hexHighlighter.RemoveRequest(HighlightRequestId.AreaHint);
                        _hexHighlighter.RequestHighlight(ranges.Select(r => r.coord), HighlightRequestId.AreaHint, HighlightVariation.AreaHint);

                        var (backRequested, input) = await GetUserInputForRequest(playerInputs.ToList(), request, subCt);
                        subCt.ThrowIfCancellationRequested();
                        _hexHighlighter.RemoveRequest(HighlightRequestId.AreaHint);

                        if (backRequested)
                        {
                            if (i == 0)
                                // Requested back at the root level -> stop the routine.
                                return (false, Enumerable.Empty<Coordinate>());

                            // take one step back -> need to offset the increment as well.
                            i -= 2;
                            _ = playerInputs.TryDequeue(out _);

                            continue;
                        }

                        if (input == null)
                        {
                            Log.Error("UI", "You should, have really, not gotten here - the only reason it's null is we've got a cancellation request in which case we probably should have thrown");
                            continue;
                        }

                        playerInputs.Enqueue(input);
                    }

                    return (true, playerInputs);
                }
                catch (OperationCanceledException)
                {
                    return (false, Enumerable.Empty<Coordinate>());
                }
                finally
                {
                    _hexHighlighter.RemoveRequest(HighlightRequestId.TargetSelection, HighlightRequestId.AreaHint);
                }
            }

            //todo: we should, really, really wait till the cancellation is done before starting the next one.
            _cts?.Cancel();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var result = await Do(_cts.Token);
            ct.ThrowIfCancellationRequested();

            return result;
        }

        public async UniTask<UnitData?> GetInputForInspection(CancellationToken ct = default)
        {
            async UniTask<UnitData?> GetUserInputForRequest(CancellationToken subCt)
            {
                try
                {
                    UnitData? inputEntity = null;
                    while (inputEntity == null)
                    {
                        await UniTask.NextFrame(subCt, true);

                        _hexHighlighter.RemoveRequest(HighlightRequestId.TileInspection);

                        var coord = FindHoveredCoordinate();
                        if (coord == null) continue;

                        _hexHighlighter.RequestHighlight(coord, HighlightRequestId.TileInspection, HighlightVariation.Normal);
                        var hoveredEntity = _map.Get<UnitData>(coord);
                        if (hoveredEntity == null) continue;

                        _hexHighlighter.RequestHighlight(coord, HighlightRequestId.TileInspection, HighlightVariation.ValidInput);

                        if (!_inputSystem.GetAction(InputMaps.Level.Interact)) continue;

                        inputEntity = hoveredEntity;
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
            var unit = await GetUserInputForRequest(_cts.Token);

            return unit;
        }

        public async UniTask<IPlayerTurnWorldSpaceInputControl.MovementInput> GetInputForMovement(UnitData mover, CancellationToken ct = default)
        {
            async UniTask<IPlayerTurnWorldSpaceInputControl.MovementInput?> GetUserInputForRequest(CancellationToken subCt)
            {
                try
                {
                    IPlayerTurnWorldSpaceInputControl.MovementInput? input = null;
                    while (!subCt.IsCancellationRequested && input == null)
                    {
                        await UniTask.NextFrame();

                        _hexHighlighter.RemoveRequest(HighlightRequestId.MovementHint);
                        var coord = FindHoveredCoordinate();
                        if (coord == null) continue;

                        var (isPathExist, path) = _pathfindingService.FindPath(mover, coord);
                        if (!isPathExist)
                        {
                            var hoveredUnit = _map.Get<UnitData>(coord);
                            if (hoveredUnit != null && hoveredUnit != mover)
                            {
                                _hexHighlighter.RequestHighlight(coord, HighlightRequestId.MovementHint, HighlightVariation.ValidInput);
                                if (_inputSystem.GetAction(InputMaps.Level.Interact)) input = new IPlayerTurnWorldSpaceInputControl.MovementInput.Inspect(hoveredUnit);
                            }
                            else
                            {
                                _hexHighlighter.RequestHighlight(coord, HighlightRequestId.MovementHint, HighlightVariation.InvalidInput);
                            }

                            continue;
                        }

                        var pathWithoutStartAndEnd = path.Except(new[] { _map.Find(mover), coord });
                        _hexHighlighter.RequestHighlight(pathWithoutStartAndEnd, HighlightRequestId.MovementHint, HighlightVariation.AreaHint);
                        _hexHighlighter.RequestHighlight(coord, HighlightRequestId.MovementHint, HighlightVariation.Normal);

                        if (!_inputSystem.GetAction(InputMaps.Level.Interact)) continue;
                        input = new IPlayerTurnWorldSpaceInputControl.MovementInput.MoveTo(coord);
                    }

                    return input;
                }
                finally
                {
                    _hexHighlighter.RemoveRequest(HighlightRequestId.MovementHint);
                }
            }

            _cts?.Cancel();
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var input = await GetUserInputForRequest(_cts.Token);
            if (input == null) return new IPlayerTurnWorldSpaceInputControl.MovementInput.Cancel();

            return input;
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
    }
}
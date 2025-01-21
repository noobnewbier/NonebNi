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
        UniTask ToTargetSelectionMode(UnitData caster, NonebAction action);
        UniTask ToTileInspectionMode();
        void UpdateTargetSelection();
    }

    public class PlayerTurnWorldSpaceInputControl : IPlayerTurnWorldSpaceInputControl
    {
        private const string TileInspectionHighlightId = "tileInspection";
        private const string TargetSelectionHighlightId = "targetSelection";
        private readonly ICoordinateAndPositionService _coordinateAndPositionService;
        private readonly Plane _gridPlane;
        private readonly IHexHighlighter _hexHighlighter;
        private readonly IInputSystem _inputSystem;
        private readonly Map _map;
        private readonly Camera _playerViewCamera;

        private CancellationTokenSource? _cts;
        private AsyncLazy? _updateLoopTask;


        public PlayerTurnWorldSpaceInputControl(IInputSystem inputSystem, ICoordinateAndPositionService coordinateAndPositionService, TerrainConfigData terrainConfigData, Camera playerViewCamera, Map map, IHexHighlighter hexHighlighter)
        {
            _inputSystem = inputSystem;
            _coordinateAndPositionService = coordinateAndPositionService;
            _playerViewCamera = playerViewCamera;
            _map = map;
            _hexHighlighter = hexHighlighter;
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

        public async UniTask ToTargetSelectionMode(UnitData caster, NonebAction action)
        {
            // _hexHighlighter.RemoveHighlight(TileInspectionHighlightId);
            //
            // _cts?.Cancel();
            //
            //
            // _cts = new CancellationTokenSource();
            // _hexHighlighter.RemoveHighlight(TargetSelectionHighlightId);
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
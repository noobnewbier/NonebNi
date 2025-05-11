using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using NonebNi.Terrain;
using NonebNi.Ui.Cameras;
using UnityEngine;
using UnityUtils;

namespace NonebNi.Main
{
    //todo: we need to organize our modules, possibly in a notion diagram, atm it's a fucking mess.
    //I suspect a-lot of our factories aren't necessary and can be reduced down to a level module/container and an ui module
    //sort this out and you can use your main sample scene to test gameplay, you are close fucker, you are v.close to something testable, keep the pressure up babe.
    public interface ILevelUi : IDisposable
    {
        void Run();
    }

    public class LevelUi : ILevelUi
    {
        private readonly CameraRunner _cameraControl;
        private readonly ICameraController _cameraController;
        private readonly CancellationTokenSource _cts;
        private readonly IGameEventControl _gameEventControl;
        private readonly Hud _hud;

        private readonly Hud.Dependencies _hudDeps;
        private readonly ITerrainMeshCreator _meshCreator;
        private readonly ISequencePlayer _sequencePlayer;
        private readonly Terrain _terrain;

        public LevelUi(
            CameraRunner cameraControl,
            Hud hud,
            Terrain terrain,
            ICameraController cameraController,
            ITerrainMeshCreator meshCreator,
            ISequencePlayer sequencePlayer,
            IGameEventControl gameEventControl,
            Hud.Dependencies hudDeps)
        {
            _meshCreator = meshCreator;
            _cameraControl = cameraControl;
            _hud = hud;
            _terrain = terrain;
            _cameraController = cameraController;
            _sequencePlayer = sequencePlayer;
            _gameEventControl = gameEventControl;
            _hudDeps = hudDeps;

            //todo: change our DI, it's confusing now.
            _cts = new CancellationTokenSource();
        }

        public void Run()
        {
            _cameraControl.Init(_cameraController);
            _hud.Init(_hudDeps);
            _terrain.Init(_meshCreator);
            _cameraControl.Run();

            ProcessLevelEvents(_cts.Token).Forget();
        }

        public void Dispose()
        {
            _cts.Cancel();
        }

        private async UniTaskVoid ProcessLevelEvents(CancellationToken ct)
        {
            await foreach (var @event in _gameEventControl.Subscribe(ct))
            {
                ct.ThrowIfCancellationRequested();
                switch (@event)
                {
                    case LevelEvent.GameStart:
                    case LevelEvent.None:
                        break;

                    case LevelEvent.WaitForActiveUnitDecision newTurn:
                        _hud.ActiveUnitControlFlow(newTurn.Unit);
                        break;

                    case LevelEvent.SequenceOccured sequenceOccured:
                        _sequencePlayer.Play(sequenceOccured.Sequences);
                        break;

                    case LevelEvent.WaitForComboDecision waitForComboDecision:
                        _hud.ComboUIFlow(waitForComboDecision.PossibleCombos);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(@event));
                }
            }
        }

        #region Camera Initialization

        public (float upBound, float downBound, float rightBound, float leftBound, float minCameraY, float maxCameraY) GetCameraParameters(
            Map map,
            ICoordinateAndPositionService coordinateAndPositionService,
            TerrainConfigData terrainConfigData,
            CameraControlSetting cameraConfig,
            Camera camera)
        {
            var coordinatePositions = map.GetAllCoordinates().Select(coordinateAndPositionService.FindPosition).ToArray();

            var (minWidth, distanceToTop, distanceToBottom) = GetViewDistanceToFrustumOnPlaneInWorldSpace(terrainConfigData, camera);
            var mapMaxHeight = coordinatePositions.DefaultIfEmpty().Max(p => p.z);
            var mapMinHeight = coordinatePositions.DefaultIfEmpty().Min(p => p.z);
            var mapMaxWidth = coordinatePositions.DefaultIfEmpty().Max(p => p.x);
            var mapMinWidth = coordinatePositions.DefaultIfEmpty().Min(p => p.x);


            var upBound = mapMaxHeight + cameraConfig.BufferToClampingEdge - distanceToTop / 2f;
            var downBound = mapMinHeight - cameraConfig.BufferToClampingEdge + distanceToBottom / 2f;
            var rightBound = mapMaxWidth + cameraConfig.BufferToClampingEdge - minWidth / 2f;
            var leftBound = mapMinWidth - cameraConfig.BufferToClampingEdge + minWidth / 2f;
            var minCameraY = cameraConfig.MinDistanceToMap + terrainConfigData.MapStartingPosition.y;
            var maxCameraY = cameraConfig.MaxDistanceToMap + terrainConfigData.MapStartingPosition.y;

            upBound = Mathf.Max(upBound, downBound);
            rightBound = Mathf.Max(rightBound, leftBound);

            return (
                upBound,
                downBound,
                rightBound,
                leftBound,
                minCameraY,
                maxCameraY
            );
        }

        /// <summary>
        ///     Finding 4 intersection point of controlledCamera frustum on the infinite plane(at mapTransform's height).
        ///     Returning the maximum size of a rect that it can be bounded within the 4 intersection point.
        /// </summary>
        private (float minWidth, float distanceToTop, float distanceToBottom) GetViewDistanceToFrustumOnPlaneInWorldSpace(TerrainConfigData terrainConfigData, Camera camera)
        {
            var cameraFrustumCorners = GetCameraFrustumCorners(camera);
            var cameraPosition = camera.transform.position;
            var targetYPosition = terrainConfigData.MapStartingPosition.y;
            var intersectionCorners = new Vector3[cameraFrustumCorners.Length];

            for (var i = 0; i < cameraFrustumCorners.Length; i++)
            {
                var (x, z) = LinearEquations.GetXzGivenY(targetYPosition, cameraPosition, cameraFrustumCorners[i]);
                intersectionCorners[i] = new Vector3(x, targetYPosition, z);
            }

            var distanceToTop = intersectionCorners[1].z - cameraPosition.z;
            var distanceToBottom = cameraPosition.z - intersectionCorners[0].z;
            var minWidth = intersectionCorners[3].x - intersectionCorners[0].x;

            return (minWidth, distanceToTop, distanceToBottom);
        }

        private Vector3[] GetCameraFrustumCorners(Camera camera)
        {
            var cameraTransform = camera.transform;
            var frustumCorners = new Vector3[4];
            camera.CalculateFrustumCorners(
                new Rect(
                    0,
                    0,
                    1,
                    1
                ),
                camera.farClipPlane,
                Camera.MonoOrStereoscopicEye.Mono,
                frustumCorners
            );

            return frustumCorners.Select(c => cameraTransform.TransformVector(c) + cameraTransform.position).ToArray();
        }

        #endregion
    }
}
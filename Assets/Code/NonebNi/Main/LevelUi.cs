using System;
using System.Linq;
using NonebNi.Core.Agents;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Terrain;
using NonebNi.Ui.Cameras;
using NonebNi.Ui.ViewComponents.PlayerTurn;
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
        private readonly Hud _hud;
        private readonly ILevelFlowController _levelFlowController;
        private readonly Terrain _terrain;

        public LevelUi(
            CameraRunner cameraControl,
            Hud hud,
            Terrain terrain,
            ICameraController cameraController,
            LevelData levelData,
            IPlayerAgent playerAgent,
            ITerrainMeshCreator meshCreator,
            IPlayerTurnWorldSpaceInputControl worldSpaceInputControl,
            ILevelFlowController levelFlowController,
            ICoordinateAndPositionService coordinateAndPositionService,
            IReadOnlyMap map,
            IUnitTurnOrderer unitTurnOrderer)
        {
            _cameraControl = cameraControl;
            _hud = hud;
            _terrain = terrain;
            _levelFlowController = levelFlowController;

            _cameraControl.Init(cameraController);
            //todo: change our DI, it's confusing now.
            _hud.Init(
                worldSpaceInputControl,
                cameraController,
                playerAgent,
                coordinateAndPositionService,
                map,
                unitTurnOrderer
            );
            _terrain.Init(meshCreator);
        }

        public void Run()
        {
            _cameraControl.Run();
        }

        public void Dispose()
        {
            _levelFlowController.NewTurnStarted -= OnNewTurnStarted;
        }

        private void OnNewTurnStarted()
        {
            _hud.RefreshForNewTurn();
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
        public (float minWidth, float distanceToTop, float distanceToBottom) GetViewDistanceToFrustumOnPlaneInWorldSpace(TerrainConfigData terrainConfigData, Camera camera)
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
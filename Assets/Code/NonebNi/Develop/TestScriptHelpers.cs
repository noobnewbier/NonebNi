using System;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Maps;
using NonebNi.Core.Units;
using NonebNi.Terrain;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace NonebNi.Develop
{
    public static class TestScriptHelpers
    {
        public static (bool success, Vector3 pos) FindMousePosInWorld(GameObject groundObject)
        {
            var mousePos = Mouse.current.position;
            var cam = Camera.main;
            if (cam == null) return default;

            var ray = cam.ScreenPointToRay(mousePos.ReadValue());
            if (!Physics.Raycast(ray, out var hit)) return default;
            if (hit.collider.gameObject != groundObject) return default;

            return (true, new Vector3(hit.point.x, groundObject.transform.position.y, hit.point.z));
        }

        public static (bool success, Vector3 pos) FindMousePosInWorld(Plane plane)
        {
            var mousePos = Mouse.current.position;
            var cam = Camera.main;
            if (cam == null) return default;

            var ray = cam.ScreenPointToRay(mousePos.ReadValue());
            if (!plane.Raycast(ray, out var distance)) return default;

            return (true, ray.GetPoint(distance));
        }

        public static void DrawGridUsingGizmos(ICoordinateAndPositionService coordinateAndPositionService, IReadOnlyMap map)
        {
            /*
             * Almost carbon copied from GridView.
             * We might want to refactor this as time goes on but for now this does the job(remember this is supposed to be throw away scripts).
             */

            var originalZTest = Handles.zTest;
            Handles.zTest = CompareFunction.Less;

            var coords = map.GetAllCoordinates();

            foreach (var coordinate in coords)
            {
                var corners = coordinateAndPositionService.FindCorners(coordinate).ToList();
                Handles.DrawLine(corners[0], corners[5]);
                for (var i = 0; i < corners.Count - 1; i++) Handles.DrawLine(corners[i], corners[i + 1]);
            }

            Handles.zTest = originalZTest;
        }

        public static UnitData CreateUnit(string unitName) =>
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
    }
}
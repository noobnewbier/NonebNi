using UnityEditor;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.Terrain
{
    [CreateAssetMenu(fileName = nameof(TerrainConfigSource), menuName = MenuName.Data + nameof(TerrainConfigSource))]
    public class TerrainConfigSource : ScriptableObject
    {
        [Range(0f, 10f)] [SerializeField] private float innerRadius;
        [SerializeField] private Vector3 mapStartingPosition = Vector3.zero;

        private static TerrainConfigSource Create(float innerRadius, Vector3 mapStartingPosition)
        {
            var toReturn = CreateInstance<TerrainConfigSource>();

            toReturn.innerRadius = innerRadius;
            toReturn.mapStartingPosition = mapStartingPosition;

            return toReturn;
        }

        public static TerrainConfigSource Create(TerrainConfigData data) =>
            Create(data.InnerRadius, data.MapStartingPosition);

        public TerrainConfigData CreateData() => new(innerRadius, mapStartingPosition);

        public void CopyFromData(TerrainConfigData terrainConfigData)
        {
            innerRadius = terrainConfigData.InnerRadius;
            mapStartingPosition = terrainConfigData.MapStartingPosition;

            EditorUtility.SetDirty(this);
        }
    }
}
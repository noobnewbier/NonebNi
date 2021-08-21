using System.Collections.Generic;
using NonebNi.Core.Level;
using NonebNi.Core.Tiles;
using UnityEngine.SceneManagement;

namespace NonebNi.Core.Maps
{
    /// <summary>
    /// Generate a <see cref="Map" /> given a <see cref="Scene" />, <see cref="MapConfigScriptable" /> and <seealso cref="WorldConfig" />
    /// Todo: we need to create a variant where the scene settings is token into account
    /// </summary>
    public class MapGenerationService
    {
        /// <summary>
        /// Create an empty map given a <see cref="MapConfigScriptable" />
        /// </summary>
        /// <returns>An empty <see cref="Map" /> with no board items, where all tiles weight is set to 1</returns>
        public Map CreateEmptyMap(MapConfigData mapConfig)
        {
            var tiles = new List<TileData>();
            for (var i = 0; i < mapConfig.GetMap2DActualHeight(); i++)
            for (var j = 0; j < mapConfig.GetMap2DActualWidth(); j++)
            {
                var tile = new TileData("TEMP_DEFAULT_NAME", 1);
                tiles.Add(tile);
            }

            return new Map(tiles, mapConfig);
        }
    }
}
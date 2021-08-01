using System.Collections.Generic;
using System.Threading.Tasks;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Tiles;
using UnityEngine.SceneManagement;

namespace NonebNi.Core.Maps
{
    /// <summary>
    /// Generate a <see cref="Map"/> given a <see cref="Scene"/>, <see cref="MapConfig"/> and <seealso cref="Core.Maps.WorldConfig"/>
    /// </summary>
    public class MapGenerationService
    {
        public async Task<Map> CreateMapAsync(MapConfig mapConfig, Scene scene)
        {
            var task = Task.Run(() => CreateMap(mapConfig, scene));

            await task;

            return task.Result;
        }

        public Map CreateMap(MapConfig mapConfig, Scene scene)
        {
            var tiles = new List<Tile>();
            for (var i = 0; i < mapConfig.GetMap2DActualHeight(); i++)
            {
                for (var j = 0; j < mapConfig.GetMap2DActualWidth(); j++)
                {
                    var tile = new Tile(new TileData("TEMP_DEFAULT_NAME", 1), new Coordinate(j, i));
                    tiles.Add(tile);
                }
            }

            return new Map(tiles, mapConfig);
        }
    }
}
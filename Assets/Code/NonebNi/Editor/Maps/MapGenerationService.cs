using System.Collections.Generic;
using System.Threading.Tasks;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using NonebNi.Core.Tiles;
using UnityEngine.SceneManagement;

namespace NonebNi.Editor.Maps
{
    /// <summary>
    /// Generate a <see cref="Map"/> given a <see cref="Scene"/>, <see cref="MapConfig"/> and <seealso cref="Core.Maps.WorldConfig"/>
    /// </summary>
    public class MapGenerationService
    {
        public async Task<Map> CreateMap(WorldConfig worldConfig, MapConfig mapConfig, Scene scene)
        {
            var task = Task.Run(
                () =>
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
            );

            await task;

            return task.Result;
        }
    }
}
using NonebNi.Game.Di;
using NonebNi.Game.Huds;
using NonebNi.Game.Level;
using UnityEngine;
using Grid = NonebNi.Game.Grids.Grid;

namespace NonebNi.Game
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private LevelDataSource levelDataSource = null!;

        [SerializeField] private Hud hud = null!;
        [SerializeField] private Grid grid = null!;


        private void Awake()
        {
            var module = new GameModule(levelDataSource.GetData());

            hud.Init(new HudComponent(module));
            grid.Init(new GridComponent(module));
        }
    }
}
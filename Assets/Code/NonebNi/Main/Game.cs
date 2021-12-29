using NonebNi.Core.Level;
using NonebNi.Core.Units;
using NonebNi.Ui.Di;
using NonebNi.Ui.Huds;
using NonebNi.Ui.Statistics.Unit;
using UnityEngine;
using Grid = NonebNi.Ui.Grids.Grid;

namespace NonebNi.Main
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private LevelDataSource levelDataSource = null!;

        [SerializeField] private Hud hud = null!;
        [SerializeField] private Grid grid = null!;
        [SerializeField] private UnitDetailStat unitDetailStat = null!;

        [SerializeField] private UnitData unitData; //temp for testing

        private void Awake()
        {
            var module = new GameModule(levelDataSource.GetData());

            hud.Init(new HudComponent(module));
            grid.Init(new GridComponent(module));

            unitDetailStat.Init();
            unitDetailStat.statView.Show(unitData);
        }
    }
}
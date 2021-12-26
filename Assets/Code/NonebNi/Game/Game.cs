using NonebNi.Game.Di;
using NonebNi.Game.Huds;
using NonebNi.Game.Level;
using UnityEngine;

namespace NonebNi.Game
{
    public class Game : MonoBehaviour
    {
        [SerializeField] private Hud hud = null!;
        [SerializeField] private LevelDataSource levelDataSource = null!;

        private void Awake()
        {
            var module = new GameModule(levelDataSource.GetData());
            hud.Init(module);
        }
    }
}
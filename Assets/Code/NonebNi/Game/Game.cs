using Code.NonebNi.Game.Di;
using Code.NonebNi.Game.Huds;
using Code.NonebNi.Game.Level;
using UnityEngine;

namespace Code.NonebNi.Game
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
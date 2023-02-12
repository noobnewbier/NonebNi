using NonebNi.Core.Agents;
using NonebNi.Core.Level;
using NonebNi.Ui.Huds;
using UnityEngine;
using UnityEngine.Serialization;

namespace NonebNi.Main
{
    public class Hud : MonoBehaviour
    {
        [FormerlySerializedAs("levelNameView")] [SerializeField]
        private PrototypeView prototypeView = null!;


        public void Init(LevelData levelData, IPlayerAgent playerAgent)
        {
            prototypeView.Init(levelData, playerAgent);
        }
    }
}
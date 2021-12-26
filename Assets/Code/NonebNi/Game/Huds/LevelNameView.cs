using NonebNi.Game.Di;
using TMPro;
using UnityEngine;

namespace NonebNi.Game.Huds
{
    /// <summary>
    /// Not intended for usage in production. Just to show something quick here
    /// </summary>
    public class LevelNameView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelNameTextMesh = null!;

        public void Init(IHudComponent hudComponent)
        {
            levelNameTextMesh.text = hudComponent.GetLevelData().LevelName;
        }
    }
}
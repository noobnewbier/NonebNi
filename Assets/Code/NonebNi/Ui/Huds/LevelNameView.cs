using NonebNi.Ui.Di;
using TMPro;
using UnityEngine;

namespace NonebNi.Ui.Huds
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
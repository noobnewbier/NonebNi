using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NonebNi.Ui.Huds
{
    /// <summary>
    /// Not intended for usage in production. Just to show something quick here
    /// </summary>
    public class PrototypeView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelNameTextMesh = null!;
        [SerializeField] private Button endTurnButton = null!;


        public void Init(LevelData levelData, ILevelFlowController levelFlowController)
        {
            levelNameTextMesh.text = levelData.LevelName;
            endTurnButton.onClick.AddListener(levelFlowController.EndTurn);
        }
    }
}
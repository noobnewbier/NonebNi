using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace NonebNi.Ui.Tooltips
{
    public class TextTooltip : TooltipBehaviour<TooltipRequest.Text>
    {
        [SerializeField] private TextMeshProUGUI textComponent = null!;

        protected override UniTask OnPopulate(TooltipRequest.Text request)
        {
            textComponent.text = request.Message.GetLocalized();
            return UniTask.CompletedTask;
        }
    }
}
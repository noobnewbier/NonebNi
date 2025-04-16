using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.Element;
using NonebNi.Core.Units;
using TMPro;
using UnityEngine;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    public class ActOrderWidget : MonoBehaviour, IElementComponent
    {
        [SerializeField] private TextMeshProUGUI unitName = null!;
        [SerializeField] private NonebButton button = null!;

        private UnitData? _unitData;

        public UniTask OnInit()
        {
            button.Clicked += OnButtonClicked;
            return UniTask.CompletedTask;
        }

        public event Action<UnitData>? Clicked;

        public async UniTask Show(int order, UnitData unitData, CancellationToken ct = default)
        {
            _unitData = unitData;
            unitName.text = $"{order}. {unitData.Name}";
        }

        private void OnButtonClicked()
        {
            if (_unitData == null) return;

            Clicked?.Invoke(_unitData);
        }
    }
}
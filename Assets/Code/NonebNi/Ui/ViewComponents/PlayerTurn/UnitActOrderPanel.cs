using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.Element;
using NonebNi.Core.Units;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    public class UnitActOrderPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panel = null!;
        [SerializeField] private ActOrderWidget widgetPrefab = null!;

        public event Action<UnitData>? UnitSelected;

        public async UniTask Show(IEnumerable<UnitData> orderedUnits, CancellationToken ct = default)
        {
            foreach (Transform child in panel.transform) Destroy(child.gameObject);

            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var tasks = new List<UniTask>();
            foreach (var unit in orderedUnits)
            {
                var (isSuccess, widget) = await NonebElement.CreateElementFromPrefab(widgetPrefab, panel.transform);
                if (!isSuccess)
                {
                    Log.Error("Failed element creation");
                    continue;
                }

                widget!.Clicked += OnWidgetClicked;
                tasks.Add(widget.Show(unit, linkedCts.Token));
            }

            await UniTask.WhenAll(tasks);
        }

        private void OnWidgetClicked(UnitData data)
        {
            UnitSelected?.Invoke(data);
        }
    }
}
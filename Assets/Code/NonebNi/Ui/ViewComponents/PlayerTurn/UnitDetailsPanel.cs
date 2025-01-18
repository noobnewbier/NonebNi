using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Units;
using UnityEngine;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    public class UnitDetailsPanel : MonoBehaviour
    {
        [SerializeField] private StatWidget healthWidget = null!;
        [SerializeField] private StatWidget fatigueWidget = null!;
        [SerializeField] private StatWidget initWidget = null!;

        public async UniTask Show(UnitData data, CancellationToken ct = default)
        {
            var (_, health) = data.Stats.FindStat("health");
            var (_, fatigue) = data.Stats.FindStat("fatigue");
            var (_, initiative) = data.Stats.FindStat("initiative");

            healthWidget.Show(health);
            fatigueWidget.Show(fatigue);
            initWidget.Show(initiative);
        }
    }
}
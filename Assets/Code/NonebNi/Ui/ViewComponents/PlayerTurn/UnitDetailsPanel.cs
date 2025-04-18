using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Stats;
using NonebNi.Core.Units;
using TMPro;
using UnityEngine;

namespace NonebNi.Ui.ViewComponents.PlayerTurn
{
    public class UnitDetailsPanel : MonoBehaviour
    {
        [SerializeField] private StatWidget healthWidget = null!;
        [SerializeField] private StatWidget fatigueWidget = null!;
        [SerializeField] private StatWidget initWidget = null!;
        [SerializeField] private StatWidget speedWidget = null!;
        [SerializeField] private StatWidget actionPointWidget = null!;
        [SerializeField] private TextMeshProUGUI nameLabel = null!;
        [SerializeField] private TextMeshProUGUI factionLabel = null!;

        public async UniTask Show(UnitData data, CancellationToken ct = default)
        {
            var (_, health) = data.Stats.FindStat(StatId.Health);
            var (_, fatigue) = data.Stats.FindStat(StatId.Fatigue);
            var (_, initiative) = data.Stats.FindStat(StatId.Initiative);
            var (_, speed) = data.Stats.FindStat(StatId.Speed);
            var (_, actionPoint) = data.Stats.FindStat(StatId.ActionPoint);

            nameLabel.text = data.Name.GetLocalized();
            factionLabel.text = data.FactionId;
            healthWidget.Show(health);
            fatigueWidget.Show(fatigue);
            initWidget.Show(initiative);
            actionPointWidget.Show(actionPoint);
            speedWidget.Show(speed);
        }
    }
}
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

        public UnitData? ShownUnit { get; private set; }

        public void Show(UnitData data)
        {
            //TODO: need unsubscribe before exiting
            if (ShownUnit != null) ShownUnit.Stats.StatUpdated -= OnStatUpdated;

            ShownUnit = data;
            ShownUnit.Stats.StatUpdated += OnStatUpdated;
            Refresh();
        }

        private void OnStatUpdated(Stat stat)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (ShownUnit == null) return;

            var (_, health) = ShownUnit.Stats.FindStat(StatId.Health);
            var (_, fatigue) = ShownUnit.Stats.FindStat(StatId.Fatigue);
            var (_, initiative) = ShownUnit.Stats.FindStat(StatId.Initiative);
            var (_, speed) = ShownUnit.Stats.FindStat(StatId.Speed);
            var (_, actionPoint) = ShownUnit.Stats.FindStat(StatId.ActionPoint);

            nameLabel.text = ShownUnit.Name.GetLocalized();
            factionLabel.text = ShownUnit.FactionId;
            healthWidget.Show(health);
            fatigueWidget.Show(fatigue);
            initWidget.Show(initiative);
            actionPointWidget.Show(actionPoint);
            speedWidget.Show(speed);
        }
    }
}
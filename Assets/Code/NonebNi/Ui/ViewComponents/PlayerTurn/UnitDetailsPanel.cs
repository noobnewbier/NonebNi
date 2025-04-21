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

        private UnitData? _currentUnit;

        public void Show(UnitData data)
        {
            //TODO: need unsubscribe before exiting
            if (_currentUnit != null) _currentUnit.Stats.StatUpdated -= OnStatUpdated;

            _currentUnit = data;
            _currentUnit.Stats.StatUpdated += OnStatUpdated;
            Refresh();
        }

        private void OnStatUpdated(Stat stat)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (_currentUnit == null) return;

            var (_, health) = _currentUnit.Stats.FindStat(StatId.Health);
            var (_, fatigue) = _currentUnit.Stats.FindStat(StatId.Fatigue);
            var (_, initiative) = _currentUnit.Stats.FindStat(StatId.Initiative);
            var (_, speed) = _currentUnit.Stats.FindStat(StatId.Speed);
            var (_, actionPoint) = _currentUnit.Stats.FindStat(StatId.ActionPoint);

            nameLabel.text = _currentUnit.Name.GetLocalized();
            factionLabel.text = _currentUnit.FactionId;
            healthWidget.Show(health);
            fatigueWidget.Show(fatigue);
            initWidget.Show(initiative);
            actionPointWidget.Show(actionPoint);
            speedWidget.Show(speed);
        }
    }
}
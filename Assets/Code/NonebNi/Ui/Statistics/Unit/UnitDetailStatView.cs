using System.Collections;
using NonebNi.Core.Units;
using NonebNi.Ui.Statistics.Skill;
using UnityEngine;
using UnityEngine.UI;

namespace NonebNi.Ui.Statistics.Unit
{
    public interface IUnitDetailStatView
    {
        Coroutine Show(UnitData unitData);
    }

    public class UnitDetailStatView : MonoBehaviour, IUnitDetailStatView
    {
        private HealthBarView _healthBarView = null!;
        private Image _image = null!;
        private Transform _skillPanelRoot = null!;
        private SkillView _skillViewPrefab = null!;

        public Coroutine Show(UnitData unitData)
        {
            IEnumerator Coroutine()
            {
                _image.sprite = unitData.Icon;
                _healthBarView.Show(unitData.Health, unitData.MaxHealth);

                foreach (Transform child in _skillPanelRoot) Destroy(child.gameObject);

                foreach (var data in unitData.SkillDatas)
                {
                    var skillView = Instantiate(_skillViewPrefab, _skillPanelRoot);
                    skillView.Show(data);
                }

                yield break;
            }

            return StartCoroutine(Coroutine());
        }

        public void Init(
            Transform skillPanelRoot,
            Image image,
            HealthBarView healthBarView,
            SkillView skillViewPrefab)
        {
            _skillPanelRoot = skillPanelRoot;
            _image = image;
            _healthBarView = healthBarView;
            _skillViewPrefab = skillViewPrefab;
        }
    }
}
using System;
using UnityEngine;

namespace NonebNi.Core.Units.Skills
{
    [Serializable]
    public class SkillData
    {
        [SerializeField] private string name;
        [SerializeField] private int cooldown;
        [SerializeField] private Sprite icon;

        public SkillData(string name, int cooldown, Sprite icon)
        {
            this.name = name;
            this.cooldown = cooldown;
            this.icon = icon;
        }

        public string Name => name;

        public int Cooldown => cooldown;

        public Sprite Icon => icon;

        public int TurnsBeforeAvailable { get; private set; }
    }
}
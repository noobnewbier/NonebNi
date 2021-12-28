using System;
using UnityEngine;

namespace NonebNi.Core.Units.Skills
{
    [Serializable]
    public class SkillData
    {
        public SkillData(string name, int cooldown, Sprite icon)
        {
            Name = name;
            Cooldown = cooldown;
            Icon = icon;
        }

        public string Name { get; }

        public int Cooldown { get; }

        public Sprite Icon { get; }

        public int TurnsBeforeAvailable { get; private set; }
    }
}
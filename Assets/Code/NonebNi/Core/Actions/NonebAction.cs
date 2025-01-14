using System;
using Noneb.Localization.Runtime;
using NonebNi.Core.Effects;
using UnityEngine;

namespace NonebNi.Core.Actions
{
    [Serializable]
    public class NonebAction
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public TargetRestriction[] TargetRestrictions { get; private set; }
        [field: SerializeField] public TargetArea TargetArea { get; private set; }
        [field: SerializeField] public int FatigueCost { get; private set; }
        [field: SerializeReference] public Effect[] Effects { get; private set; }
        [field: SerializeReference] public Range[] Ranges { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public NonebLocString Name { get; private set; }

        public NonebAction(
            string id,
            Range range,
            TargetRestriction[] targetRestrictions,
            TargetArea targetArea,
            int fatigueCost,
            Sprite icon,
            NonebLocString name,
            params Effect[] effects) :
            this(
                id,
                new[] { range },
                targetRestrictions,
                targetArea,
                fatigueCost,
                icon,
                name,
                effects
            ) { }

        public NonebAction(
            string id,
            Range[] ranges,
            TargetRestriction[] targetRestrictions,
            TargetArea targetArea,
            int fatigueCost,
            Sprite icon,
            NonebLocString name,
            params Effect[] effects)
        {
            Id = id;
            Ranges = ranges;
            TargetRestrictions = targetRestrictions;
            TargetArea = targetArea;
            FatigueCost = fatigueCost;
            Icon = icon;
            Name = name;
            Effects = effects;
        }

        public NonebAction(
            string id,
            Range range,
            TargetRestriction targetRestriction,
            TargetArea targetArea,
            int fatigueCost,
            Sprite icon,
            NonebLocString name,
            params Effect[] effects)
            : this(
                id,
                new[] { range },
                new[] { targetRestriction },
                targetArea,
                fatigueCost,
                icon,
                name,
                effects
            ) { }

        public NonebAction(
            string id,
            Range range,
            TargetRestriction[] targetRestrictions,
            TargetArea targetArea,
            int fatigueCost,
            params Effect[] effects) :
            this(
                id,
                new[] { range },
                targetRestrictions,
                targetArea,
                fatigueCost,
                effects
            ) { }

        public NonebAction(
            string id,
            Range[] ranges,
            TargetRestriction[] targetRestrictions,
            TargetArea targetArea,
            int fatigueCost,
            params Effect[] effects)
        {
            Id = id;
            Ranges = ranges;
            TargetRestrictions = targetRestrictions;
            TargetArea = targetArea;
            FatigueCost = fatigueCost;
            Icon = Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.zero);
            Name = NonebLocString.Default;
            Effects = effects;
        }

        public NonebAction(
            string id,
            Range range,
            TargetRestriction targetRestriction,
            TargetArea targetArea,
            int fatigueCost,
            params Effect[] effects)
            : this(
                id,
                new[] { range },
                new[] { targetRestriction },
                targetArea,
                fatigueCost,
                effects
            ) { }

        public override string ToString() => Id;
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Noneb.Localization.Runtime;
using NonebNi.Core.Effects;
using UnityEngine;

namespace NonebNi.Core.Actions
{
    [Serializable]
    public class NonebAction
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public int FatigueCost { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public NonebLocString Name { get; private set; }
        [field: SerializeField] public TargetRequest[] TargetRequests { get; private set; } = Array.Empty<TargetRequest>();
        [field: SerializeReference] public Effect[] Effects { get; private set; } = Array.Empty<Effect>(); //todo:...?

        public NonebAction(
            string id,
            NonebLocString name,
            Sprite icon,
            int fatigueCost,
            IEnumerable<TargetRequest> targetRequirements,
            IEnumerable<Effect> effects)
        {
            Id = id;
            FatigueCost = fatigueCost;
            Icon = icon;
            Name = name;
            TargetRequests = targetRequirements.ToArray();
            Effects = effects.ToArray();
        }

        public NonebAction(
            string id,
            NonebLocString name,
            Sprite icon,
            int fatigueCost,
            IEnumerable<TargetRequest> targetRequirements,
            params Effect[] effects) :
            this(
                id,
                name,
                icon,
                fatigueCost,
                targetRequirements,
                effects.AsEnumerable()
            ) { }

        public NonebAction(
            string id,
            NonebLocString name,
            Sprite icon,
            int fatigueCost,
            TargetRequest targetRequest,
            params Effect[] effects) :
            this(
                id,
                name,
                icon,
                fatigueCost,
                new[] { targetRequest },
                effects.AsEnumerable()
            ) { }

        public NonebAction(
            string id,
            NonebLocString name,
            Sprite icon,
            int fatigueCost,
            Range range,
            TargetArea targetArea,
            IEnumerable<TargetRestriction> targetRestrictions,
            params Effect[] effects) :
            this(
                id,
                name,
                icon,
                fatigueCost,
                targetRestrictions.Select(r => new TargetRequest(r, targetArea, range)),
                effects
            ) { }

        public NonebAction(
            string id,
            NonebLocString name,
            Sprite icon,
            int fatigueCost,
            Range range,
            TargetArea targetArea,
            TargetRestriction targetRestriction,
            params Effect[] effects)
            : this(
                id,
                name,
                icon,
                fatigueCost,
                new[]
                {
                    new TargetRequest(targetRestriction, targetArea, range)
                },
                effects
            ) { }

        public NonebAction(
            string id,
            Sprite icon,
            int fatigueCost,
            TargetRequest targetRequest,
            params Effect[] effects) : this(
            id,
            $"NAMELESS_{id}",
            icon,
            fatigueCost,
            new[] { targetRequest },
            effects
        ) { }

        public NonebAction(string id, int fatigueCost, Range range, TargetArea area, TargetRestriction restriction, params Effect[] effects) : this(
            id,
            Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.zero),
            fatigueCost,
            new TargetRequest(
                restriction,
                area,
                range
            ),
            effects
        ) { }

        public override string ToString() => Id;
    }
}
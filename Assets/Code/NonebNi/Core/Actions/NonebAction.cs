using System;
using System.Collections.Generic;
using System.Linq;
using Noneb.Localization.Runtime;
using NonebNi.Core.Effects;
using NonebNi.Core.Stats;
using UnityEngine;

namespace NonebNi.Core.Actions
{
    [Serializable]
    public class NonebAction
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public StatCost[] Costs { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public NonebLocString Name { get; private set; }
        [field: SerializeField] public TargetRequest[] TargetRequests { get; private set; }
        [field: SerializeReference] public Effect[] Effects { get; private set; }

        public NonebAction(string id, StatCost[] costs, Sprite icon, NonebLocString name, TargetRequest[] targetRequests, Effect[] effects)
        {
            Id = id;
            Costs = costs;
            Icon = icon;
            Name = name;
            TargetRequests = targetRequests;
            Effects = effects;
        }

        public NonebAction(
            string id,
            NonebLocString name,
            Sprite icon,
            int fatigueCost,
            int actionPointCost,
            IEnumerable<TargetRequest> targetRequirements,
            IEnumerable<Effect> effects) :
            this(
                id,
                new StatCost[]
                {
                    new(StatId.Fatigue, fatigueCost),
                    new(StatId.ActionPoint, actionPointCost)
                },
                icon,
                name,
                targetRequirements.ToArray(),
                effects.ToArray()
            ) { }

        public NonebAction(
            string id,
            NonebLocString name,
            Sprite icon,
            int fatigueCost,
            int actionPointCost,
            IEnumerable<TargetRequest> targetRequirements,
            params Effect[] effects) :
            this(
                id,
                name,
                icon,
                fatigueCost,
                actionPointCost,
                targetRequirements,
                effects.AsEnumerable()
            ) { }

        public NonebAction(
            string id,
            NonebLocString name,
            Sprite icon,
            int fatigueCost,
            int actionPointCost,
            TargetRequest targetRequest,
            params Effect[] effects) :
            this(
                id,
                name,
                icon,
                fatigueCost,
                actionPointCost,
                new[] { targetRequest },
                effects.AsEnumerable()
            ) { }

        public NonebAction(
            string id,
            NonebLocString name,
            Sprite icon,
            int fatigueCost,
            int actionPointCost,
            Range range,
            TargetArea targetArea,
            IEnumerable<TargetRestriction> targetRestrictions,
            params Effect[] effects) :
            this(
                id,
                name,
                icon,
                fatigueCost,
                actionPointCost,
                targetRestrictions.Select(r => new TargetRequest(r, targetArea, range)),
                effects
            ) { }

        public NonebAction(
            string id,
            NonebLocString name,
            Sprite icon,
            int fatigueCost,
            int actionPointCost,
            Range range,
            TargetArea targetArea,
            TargetRestriction targetRestriction,
            params Effect[] effects)
            : this(
                id,
                name,
                icon,
                fatigueCost,
                actionPointCost,
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
            int actionPointCost,
            TargetRequest targetRequest,
            params Effect[] effects) : this(
            id,
            $"NAMELESS_{id}",
            icon,
            fatigueCost,
            actionPointCost,
            new[] { targetRequest },
            effects
        ) { }

        public NonebAction(string id, int fatigueCost, int actionPointCost, Range range, TargetArea area, TargetRestriction restriction, params Effect[] effects) : this(
            id,
            Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.zero),
            fatigueCost,
            actionPointCost,
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
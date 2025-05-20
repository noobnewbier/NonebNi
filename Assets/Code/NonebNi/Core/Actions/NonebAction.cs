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
        [field: SerializeField] public NonebLocString Name { get; private set; }
        [field: SerializeField] public Sprite Icon { get; private set; }
        [field: SerializeField] public StatCost[] Costs { get; private set; }
        [field: SerializeField] public TargetRequest[] TargetRequests { get; private set; }

        /// <summary>
        /// Note the order here is important,
        /// e.g knock back before damage is applied means the damage might not be applied if the unit is now not in its original
        /// position.
        /// </summary>
        [field: SerializeReference]
        public Effect[] Effects { get; private set; }

        [field: SerializeField] public bool IsComboStarter { get; private set; }

        public NonebAction(string id, NonebLocString name, Sprite icon, StatCost[] costs, TargetRequest[] targetRequests, Effect[] effects, bool isComboStarter)
        {
            Id = id;
            Costs = costs;
            Icon = icon;
            Name = name;
            TargetRequests = targetRequests;
            Effects = effects;
            IsComboStarter = isComboStarter;
        }

        public NonebAction(
            string id,
            NonebLocString name,
            Sprite icon,
            int fatigueCost,
            int actionPointCost,
            IEnumerable<TargetRequest> targetRequirements,
            IEnumerable<Effect> effects,
            bool isComboStarter) :
            this(
                id,
                name,
                icon,
                new StatCost[]
                {
                    new(StatId.Fatigue, fatigueCost),
                    new(StatId.ActionPoint, actionPointCost)
                },
                targetRequirements.ToArray(),
                effects.ToArray(),
                isComboStarter
            ) { }

        public NonebAction(
            string id,
            NonebLocString name,
            Sprite icon,
            int fatigueCost,
            int actionPointCost,
            IEnumerable<TargetRequest> targetRequirements,
            bool isComboStarter,
            params Effect[] effects) :
            this(
                id,
                name,
                icon,
                fatigueCost,
                actionPointCost,
                targetRequirements,
                effects.AsEnumerable(),
                isComboStarter
            ) { }

        public NonebAction(
            string id,
            NonebLocString name,
            Sprite icon,
            int fatigueCost,
            int actionPointCost,
            TargetRequest targetRequest,
            bool isComboStarter,
            params Effect[] effects) :
            this(
                id,
                name,
                icon,
                fatigueCost,
                actionPointCost,
                new[] { targetRequest },
                effects.AsEnumerable(),
                isComboStarter
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
            bool isComboStarter,
            params Effect[] effects) :
            this(
                id,
                name,
                icon,
                fatigueCost,
                actionPointCost,
                targetRestrictions.Select(r => new TargetRequest(r, targetArea, range)),
                isComboStarter,
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
            bool isComboStarter,
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
                isComboStarter,
                effects
            ) { }

        public NonebAction(
            string id,
            Sprite icon,
            int fatigueCost,
            int actionPointCost,
            TargetRequest targetRequest,
            bool isComboStarter,
            params Effect[] effects) : this(
            id,
            $"NAMELESS_{id}",
            icon,
            fatigueCost,
            actionPointCost,
            new[] { targetRequest },
            isComboStarter,
            effects
        ) { }

        public NonebAction(string id, int fatigueCost, int actionPointCost, Range range, TargetArea area, TargetRestriction restriction, bool isComboStarter, params Effect[] effects) : this(
            id,
            Sprite.Create(Texture2D.blackTexture, Rect.zero, Vector2.zero),
            fatigueCost,
            actionPointCost,
            new TargetRequest(
                restriction,
                area,
                range
            ),
            isComboStarter,
            effects
        ) { }

        public override string ToString() => Id;
    }
}
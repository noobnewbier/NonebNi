using System;
using Noneb.Localization.Runtime;
using NonebNi.Core.Actions;
using NonebNi.Core.Effects;
using UnityEngine;
using UnityUtils.Constants;
using Range = NonebNi.Core.Actions.Range;

namespace NonebNi.EditorComponent.Entities.Skills
{
    /// <summary>
    ///     Temporary implementation to fill in the gaps - to be removed
    /// </summary>
    [CreateAssetMenu(fileName = nameof(NonebActionDataSource), menuName = MenuName.Data + nameof(NonebAction))]
    public class NonebActionDataSource : ScriptableObject
    {
        //TODO: some of this won't serialize(editable in Unity) properly atm. Considering porting data editor.

        [field: SerializeField] public string Id { get; private set; } = string.Empty;
        [field: SerializeField] public TargetRestriction[] TargetRestrictions { get; private set; } = Array.Empty<TargetRestriction>();
        [field: SerializeField] public TargetArea TargetArea { get; private set; }
        [field: SerializeField] public int FatigueCost { get; private set; }
        [field: SerializeReference] public Effect[] Effects { get; private set; } = Array.Empty<Effect>();
        [field: SerializeReference] public Range[] Ranges { get; private set; } = Array.Empty<Range>();
        [field: SerializeField] public Sprite? Icon { get; private set; }
        [field: SerializeField] public NonebLocString Name { get; private set; } = string.Empty;

        public NonebAction CreateData() => new(
            Id,
            Ranges,
            TargetRestrictions,
            TargetArea,
            FatigueCost,
            Icon ?? Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
            Name,
            Effects
        );
    }
}
using System;
using Noneb.Localization.Runtime;
using NonebNi.Core.Actions;
using NonebNi.Core.Effects;
using UnityEngine;
using UnityUtils.Constants;

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
        [field: SerializeField] public int FatigueCost { get; private set; }
        [field: SerializeField] public int ActionPointCost { get; private set; }
        [field: SerializeReference] public Effect[] Effects { get; private set; } = Array.Empty<Effect>();
        [field: SerializeField] public Sprite? Icon { get; private set; }
        [field: SerializeField] public NonebLocString Name { get; private set; } = string.Empty;
        [field: SerializeField] public TargetRequest[] Requirements { get; private set; } = Array.Empty<TargetRequest>();


        public NonebAction CreateData() =>
            new(
                Id,
                Name,
                Icon ?? Sprite.Create(Texture2D.whiteTexture, Rect.zero, Vector2.zero),
                FatigueCost,
                ActionPointCost,
                Requirements,
                Effects
            );
    }
}
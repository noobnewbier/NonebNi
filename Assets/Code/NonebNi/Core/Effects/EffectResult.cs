using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Entities;
using NonebNi.Core.Sequences;

namespace NonebNi.Core.Effects
{
    /// <summary>
    /// Combo is valid when either the target is in <see cref="ValidComboReceiver" />, or if the action is initiated by
    /// <see cref="ValidComboReceiver" />
    /// </summary>
    public record EffectResult(IEnumerable<ISequence> Sequences, ISet<IActionTarget> ValidComboReceiver, ISet<EntityData> ValidComboCarrier)
    {
        public EffectResult(IEnumerable<ISequence> Sequences) : this(Sequences, new HashSet<IActionTarget>(), new HashSet<EntityData>()) { }
        public static EffectResult Empty { get; } = new(Enumerable.Empty<ISequence>());
        public bool CanCombo => ValidComboReceiver.Any() || ValidComboCarrier.Any();

        public EffectResult Concat(EffectResult result) =>
            new(
                Sequences.Concat(result.Sequences),
                ValidComboReceiver.Union(result.ValidComboReceiver).ToHashSet(),
                ValidComboCarrier.Union(result.ValidComboCarrier).ToHashSet()
            );
    }
}
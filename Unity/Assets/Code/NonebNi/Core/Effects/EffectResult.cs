using System;
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
    public record EffectResult(IReadOnlyList<ISequence> Sequences, ISet<IActionTarget> ValidComboReceiver, ISet<EntityData> ValidComboCarrier)
    {
        public EffectResult(IEnumerable<ISequence> Sequences, ISet<IActionTarget> ValidComboReceiver, ISet<EntityData> ValidComboCarrier) : this(Sequences.ToArray(), ValidComboReceiver, ValidComboCarrier) { }
        public EffectResult(IReadOnlyList<ISequence> Sequences) : this(Sequences, new HashSet<IActionTarget>(), new HashSet<EntityData>()) { }
        public EffectResult(IEnumerable<ISequence> Sequences) : this(Sequences.ToArray()) { }

        public static EffectResult Empty { get; } = new (ArraySegment<ISequence>.Empty);
        public bool CanCombo => ValidComboReceiver.Any() || ValidComboCarrier.Any();

        public EffectResult Concat(EffectResult result) =>
            new
            (
                Sequences.Concat(result.Sequences).ToArray(),
                ValidComboReceiver.Union(result.ValidComboReceiver).ToHashSet(),
                ValidComboCarrier.Union(result.ValidComboCarrier).ToHashSet()
            );
    }
}
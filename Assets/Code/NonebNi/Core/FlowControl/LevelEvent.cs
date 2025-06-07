using System.Collections.Generic;
using NonebNi.Core.Commands;
using NonebNi.Core.Effects;
using NonebNi.Core.Units;

namespace NonebNi.Core.FlowControl
{
    public abstract record LevelEvent
    {
        public record None : LevelEvent;

        public record GameStart : LevelEvent;

        public record WaitForActiveUnitDecision(UnitData Unit) : LevelEvent;

        public record WaitForComboDecision(IEnumerable<ICommand> PossibleCombos) : LevelEvent;

        public record SequenceOccured(EffectResult Result) : LevelEvent;
    }
}
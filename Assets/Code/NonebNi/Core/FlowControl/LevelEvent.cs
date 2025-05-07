using System.Collections.Generic;
using NonebNi.Core.Commands;
using NonebNi.Core.Sequences;
using NonebNi.Core.Units;

namespace NonebNi.Core.FlowControl
{
    public abstract record LevelEvent
    {
        public record None : LevelEvent;

        public record GameStart : LevelEvent;

        public record WaitForActiveUnitDecision(UnitData Unit) : LevelEvent;

        public record WaitForComboDecision(UnitData Unit, IEnumerable<ICommand> PossibleCombos) : LevelEvent;

        public record SequenceOccured(IEnumerable<ISequence> Sequences) : LevelEvent;
    }
}
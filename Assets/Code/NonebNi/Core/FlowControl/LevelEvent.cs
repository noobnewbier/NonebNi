using System.Collections.Generic;
using NonebNi.Core.Commands;
using NonebNi.Core.Sequences;
using NonebNi.Core.Units;

namespace NonebNi.Core.FlowControl
{
    public abstract record LevelEvent
    {
        public record NewTurn(UnitData Unit) : LevelEvent;

        public record WaitForComboDecision(UnitData Unit, IEnumerable<ICommand> PossibleCombos) : LevelEvent;

        public record SequenceOccured(IEnumerable<ISequence> Sequences) : LevelEvent;
    }
}
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Maps;
using NonebNi.Core.Units;
using Priority_Queue;

namespace NonebNi.Core.FlowControl
{
    public interface IUnitTurnManager
    {
        UnitData ToNextUnit();
    }

    /// <summary>
    /// Initiative base, all units must have took one turn before others can go again
    /// </summary>
    public class UnitTurnOrderer : IUnitTurnManager
    {
        private readonly IReadOnlyMap _map;

        private readonly SimplePriorityQueue<UnitData> _unitInOrder;

        public UnitTurnOrderer(IReadOnlyMap map)
        {
            _map = map;
            _unitInOrder = new SimplePriorityQueue<UnitData>(
                //the priority queue prioritize elements with lower priority value, we want the opposite
                Comparer<float>.Create((a, b) => -a.CompareTo(b))
            );
        }

        public UnitData ToNextUnit()
        {
            if (!_unitInOrder.Any())
                foreach (var unitData in _map.GetAllUnits())
                    _unitInOrder.Enqueue(unitData, unitData.Initiative);

            return _unitInOrder.Dequeue();
        }
    }
}
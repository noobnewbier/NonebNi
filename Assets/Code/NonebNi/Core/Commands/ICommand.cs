using System.Collections.Generic;
using NonebNi.Core.Units;

namespace NonebNi.Core.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// Evaluate the command and modify unit accordingly
        /// </summary>
        /// <returns>All affected <see cref="UnitData" /> after evaluating the command</returns>
        IEnumerable<UnitData> Evaluate();
    }
}
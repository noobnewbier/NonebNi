using System.Linq;
using UnityEngine;

namespace NonebNi.Core.Actions
{
    public interface IActionRepository
    {
        NonebAction? GetAction(string actionId);
    }

    public class ActionRepository : IActionRepository
    {
        private readonly NonebAction[] _actions;

        public ActionRepository(NonebAction[] actions)
        {
            if (actions.Distinct().Count() != actions.Length)
            {
                Debug.LogError("Multiple actions with the same ID - this is invalid");
            }

            _actions = actions;
        }

        public NonebAction? GetAction(string actionId) => _actions.FirstOrDefault(a => a.Id == actionId);
    }
}
using System.Linq;
using UnityEngine;

namespace NonebNi.Core.Actions
{
    public interface IActionRepository
    {
        Action? GetAction(string actionId);
    }

    public class ActionRepository : IActionRepository
    {
        private readonly Action[] _actions;

        public ActionRepository(Action[] actions)
        {
            if (actions.Distinct().Count() != actions.Length)
            {
                Debug.LogError("Multiple actions with the same ID - this is invalid");
            }

            _actions = actions;
        }

        public Action? GetAction(string actionId) => _actions.FirstOrDefault(a => a.Id == actionId);
    }
}
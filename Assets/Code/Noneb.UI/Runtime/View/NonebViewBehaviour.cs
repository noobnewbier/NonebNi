using System.Collections.Generic;
using Noneb.UI.Element;
using UnityEngine;

namespace Noneb.UI.View
{
    /*
     * TODO: current goal:
     * - pause menu
     * - choose action menu -> submit -> execute action
     * - inspect unit -> change chosen action
     * - combo menu(might have multiple possible combo resolution.
     * - enemy action menu
     *
     * TODO: proper input system implementation
     *
     * if we can handle all these, we should be somewhat ready
     * we might have to go a bit back and forth with the implementation and switch task at hand though, combo for example is not implemented yet.
     */

    /// <summary>
    /// At least for now, we have no plans to deal with UITK. Just focus on UGUI for now and we can migrate later once it's
    /// actually human friendly.
    /// ---
    /// Note:
    /// I've thought about making this a POCO, decided against it.
    /// The main reason is that the notion of NonebView is to reason/formulate with the UGUI element hierarchy,
    /// and it kind of make sense for it to be a MonoBehaviour for this reason -> it needs access to the game object hierarchy
    /// to work with the child/parent/siblings relationship
    /// Now we "can" make a container and somewhat manually keep track of the hierarchy change and update the POCO sides, but
    /// it's less robust and prone to the "i forgot to update view hierarchy error"
    /// Whilst now we can just query the hierarchy every single time, especially as we aren't caching stuffs given it's
    /// relatively cheap to check the hierarchy anyway.
    /// So the only real advantage to make it a POCO is that automated testing will be much faster, but we can live with
    /// slightly slower automated tests.
    /// ---
    /// </summary>
    //TODO: idk what's this missing but it's missing sth.
    //TODO: maybe view itself is a poco and we need a container for it.
    public class NonebViewBehaviour : MonoBehaviour, INonebView
    {
        Dictionary<IViewComponent, bool> INonebView.IsComponentWaked { get; set; } = new();

        public bool IsViewActive { get; private set; }

        public string Name => gameObject.name;

        INonebView.InitializationState INonebView.InitState { get; set; }

        void INonebView.SetActive(bool isActive)
        {
            IsViewActive = isActive;
        }

        IEnumerable<IViewComponent> INonebView.FindViewComponents() => GetComponents<IViewComponent>();

        IEnumerable<NonebElement> INonebView.FindChildElements()
        {
            foreach (var element in GetComponentsInChildren<NonebElement>(true))
            {
                if (element.OwnerView != this) continue;

                yield return element;
            }
        }
    }
}
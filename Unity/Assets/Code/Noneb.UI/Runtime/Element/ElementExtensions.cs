using UnityEngine;

namespace Noneb.UI.Element
{
    public static class ElementExtensions
    {
        public static NonebElement? FindOwner<T>(this T elementComponent) where T : MonoBehaviour, IElementComponent => elementComponent.GetComponentInParent<NonebElement>(true);

        public static bool IsElementActive<T>(this T elementComponent) where T : MonoBehaviour, IElementComponent
        {
            /*
             * If this is not efficient we can do caching, but it's quite error-prone as we actually need to clear/refresh cache as well
             * We will sort it out when it's a problem
             */
            if (elementComponent.FindOwner() is not { } owner) return false;

            return owner.IsElementActive;
        }
    }
}
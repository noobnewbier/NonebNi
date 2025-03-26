using UnityEngine;

namespace Noneb.UI.Element
{
    public static class ElementExtensions
    {
        public static NonebElement? FindOwner<T>(this T elementComponent) where T : MonoBehaviour, IElementComponent => elementComponent.GetComponentInParent<NonebElement>(true);
    }
}
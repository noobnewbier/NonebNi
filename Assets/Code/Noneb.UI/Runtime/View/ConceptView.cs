using System.Collections.Generic;
using System.Linq;
using Noneb.UI.Element;

namespace Noneb.UI.View
{
    //TODO: feels like a horrible idea.
    public sealed class ConceptView : INonebView
    {
        private readonly IViewComponent[] _components;

        public ConceptView(string name, params IViewComponent[] components)
        {
            _components = components;
            Name = name;
        }

        public string Name { get; }
        Dictionary<IViewComponent, bool> INonebView.IsComponentWaked { get; set; } = new();
        INonebView.InitializationState INonebView.InitState { get; set; }
        public bool IsViewActive { get; private set; }

        void INonebView.SetActive(bool isActive)
        {
            IsViewActive = isActive;
        }

        IEnumerable<IViewComponent> INonebView.FindViewComponents() => _components;

        IEnumerable<NonebElement> INonebView.FindChildElements() => Enumerable.Empty<NonebElement>();
    }
}
using Noneb.UI.View;
using NonebNi.Ui.Attributes;
using UnityEngine;

namespace NonebNi.Main
{
    [NonebUniversalEditor]
    public class CanvasRoot : MonoBehaviour
    {
        private UIStack? _stack;

        public UIStack GetStack()
        {
            _stack ??= new UIStack(this);
            return _stack;
        }
    }
}
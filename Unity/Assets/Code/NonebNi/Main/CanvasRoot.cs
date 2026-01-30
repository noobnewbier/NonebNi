using Noneb.UI.View;
using NonebNi.Core.Attributes;
using UnityEngine;

namespace NonebNi.Main
{
    [NonebUniversalInspector]
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
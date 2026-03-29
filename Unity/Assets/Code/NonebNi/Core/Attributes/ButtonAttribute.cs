using System;
using JetBrains.Annotations;

namespace NonebNi.Core.Attributes
{
    [MeansImplicitUse]
    public class ButtonAttribute : Attribute
    {
        public readonly bool RepaintScene;
        public ButtonAttribute(bool repaintScene = false)
        {
            RepaintScene = repaintScene;
        }
    }
}
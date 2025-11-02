using System;

namespace NonebNi.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CallOnEditorEnabledAttribute : Attribute
    {
        public readonly object[] Parameters;

        public CallOnEditorEnabledAttribute(params object[] parameters)
        {
            Parameters = parameters;
        }
    }
}
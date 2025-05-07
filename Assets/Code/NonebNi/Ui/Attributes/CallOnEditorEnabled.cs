using System;

namespace NonebNi.Ui.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class CallOnEditorEnabled : Attribute
    {
        public readonly object[] Parameters;

        public CallOnEditorEnabled(params object[] parameters)
        {
            Parameters = parameters;
        }
    }
}
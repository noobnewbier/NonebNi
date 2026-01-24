using System;
using System.Collections.Generic;
using UnityEngine;

namespace NonebNi.Core.Attributes
{
    public class AutoCompleteFieldAttribute : PropertyAttribute
    {
        public readonly Func<IEnumerable<string>> OptionsFactory;

        public AutoCompleteFieldAttribute(Func<IEnumerable<string>> optionsFactory)
        {
            OptionsFactory = optionsFactory;
        }

        public AutoCompleteFieldAttribute(string[] options)
        {
            OptionsFactory = () => options;
        }
    }
}
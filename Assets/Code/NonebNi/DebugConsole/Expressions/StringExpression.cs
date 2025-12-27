using System;
using System.Text.RegularExpressions;

namespace NonebNi.DebugConsole.Expressions
{
    public class StringExpression : Expression
    {
        public static readonly Regex Pattern = new("([a-zA-Z]+-*)+");

        public StringExpression(string value) : base(value)
        {
            StringValue = value;
        }

        public override Type[] ConvertableTypes => new[]
        {
            typeof(string),
            typeof(bool)

            /*
             * I can make this so it can work with DataRef, but that's a lot of type shenanigans I don't want to do now.
             * Curious about how to do it elegantly.
             */
        };

        public string StringValue { get; }

        public override object ConvertTo(Type type)
        {
            if (type == typeof(bool))
            {
                var sanitizedValue = StringValue.ToLower();
                return bool.Parse(sanitizedValue);
            }

            if (type == typeof(string)) return StringValue;

            throw new InvalidOperationException();
        }
    }
}
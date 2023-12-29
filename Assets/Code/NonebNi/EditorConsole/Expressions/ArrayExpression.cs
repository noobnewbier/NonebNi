using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NonebNi.EditorConsole.Expressions
{
    public class ArrayExpression : Expression
    {
        public static readonly Regex Pattern = new(@"\[.*\]");

        public ArrayExpression(Expression[] expressions, string stringRepresentation) : base(stringRepresentation)
        {
            var convertableTypes = expressions.Select(e => e.ConvertableType).ToArray();
            if (convertableTypes.Distinct().Count() != 1)
                throw new ArgumentException(
                    "Array can only accept element of the same type at the moment! There's a way around it but I am too lazy now"
                );

            var arrayElementType = convertableTypes.First();
            ConvertableType = arrayElementType.MakeArrayType();

            var objects = expressions.Select(v => v.Value).ToArray();
            var typedObjectsArray = Array.CreateInstance(arrayElementType, objects.Length);
            for (var i = 0; i < objects.Length; i++) typedObjectsArray.SetValue(objects[i], i);
            Value = typedObjectsArray;
        }

        public override Type ConvertableType { get; }
        public override object Value { get; }
    }
}
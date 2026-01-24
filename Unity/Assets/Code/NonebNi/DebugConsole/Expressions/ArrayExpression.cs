using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NonebNi.DebugConsole.Expressions
{
    public class ArrayExpression : Expression
    {
        public static readonly Regex Pattern = new(@"\[.*\]");
        private readonly Type _arrayElementType;
        private readonly Array value;

        public ArrayExpression(Expression[] expressions, string stringRepresentation) : base(stringRepresentation)
        {
            var typeOptionsIntersect = new HashSet<Type>(expressions.FirstOrDefault()?.ConvertableTypes ?? Array.Empty<Type>());
            foreach (var expression in expressions.Skip(1)) typeOptionsIntersect.IntersectWith(expression.ConvertableTypes);

            var convertableType = typeOptionsIntersect.FirstOrDefault();
            if (convertableType == null)
            {
                //Cannot find common type! Default to type object
                _arrayElementType = typeof(object);
                value = Array.Empty<object>();
                return;
            }

            _arrayElementType = convertableType;

            var objects = expressions.Select(v => v.ConvertTo(_arrayElementType)).ToArray();
            var typedObjectsArray = Array.CreateInstance(_arrayElementType, objects.Length);
            for (var i = 0; i < objects.Length; i++) typedObjectsArray.SetValue(objects[i], i);
            value = typedObjectsArray;
        }

        public override Type[] ConvertableTypes => new[] { _arrayElementType.MakeArrayType() };

        public override object ConvertTo(Type type)
        {
            if (!ConvertableTypes.Contains(type)) throw new ArgumentException("The specified type is not convertable");

            return value;
        }
    }
}
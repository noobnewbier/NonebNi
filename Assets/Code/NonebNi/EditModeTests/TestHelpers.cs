using System.Collections;

namespace NonebNi.EditModeTests
{
    public static class TestHelpers
    {
        public static void EvaluateEnumerable(this IEnumerable enumerable)
        {
            foreach (var _ in enumerable) { }
        }
    }
}
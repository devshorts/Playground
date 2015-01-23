using System;
using System.Linq.Expressions;

namespace NoNulls
{
    public class Option
    {
        public static MethodValue<T> Safe<T>(Expression<Func<T>> input)
        {
            var transform = (Expression<Func<MethodValue<T>>>)new NullVisitor<T>().Visit(input);

            return transform.Compile()();
        }
    }
}

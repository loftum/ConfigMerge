using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using ConfigMerge.Core.Formatting;

namespace ConfigMerge.Core.Lang
{
    public static class CustomExpression
    {
        static CustomExpression()
        {
            StringConcatMethod = typeof (string).GetMethod("Concat", new [] { typeof (object[])});
        }

        private static readonly MethodInfo StringConcatMethod;
        public static MethodCallExpression StringConcat(params Expression[] parts)
        {
            var argument = Expression.NewArrayInit(typeof (object), parts);
            return Expression.Call(StringConcatMethod, argument);
        }

        public static Expression Concat(Expression left, Expression right)
        {
            left = left.StripQuotes();
            right = right.StripQuotes();

            if (left.Type == typeof (string) && right.Type == typeof (string))
            {
                return StringConcat(left, right);
            }
            

            var leftArray = left as NewArrayExpression;
            var rightArray = right as NewArrayExpression;

            if (leftArray == null && rightArray != null)
            {
                return rightArray.Update(rightArray.Expressions.Select(e => Concat(left, e)));
            }
            if (leftArray != null && rightArray == null)
            {
                return leftArray.Update(leftArray.Expressions.Select(e => Concat(e, right)));
            }

            if (leftArray != null && leftArray.Type == rightArray.Type)
            {
                var values = leftArray.Expressions.Concat(rightArray.Expressions).ToList();
                return leftArray.Update(values);
            }
            

            throw new InvalidOperationException($"Cannot concat {left.Type.Name} and {right.Type.Name}");
        }
    }
}
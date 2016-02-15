using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ConfigMerge.Collections;
using ConfigMerge.Core.Formatting;
using ConfigMerge.Logging;

namespace ConfigMerge.Core.Lang
{
    public class RecipeParser
    {
        private static readonly ILogger Log = Logger.For<RecipeParser>();

        public static readonly ParameterExpression Transformer = Expression.Parameter(typeof(IConfigTransformer), "t");
        public static readonly MethodInfo Transform = typeof(IConfigTransformer).GetMethod("Transform");

        public Expression<Action<IConfigTransformer>> Recipe { get; }

        public RecipeParser(IEnumerable<Token> tokens)
        {
            var enumerator = tokens.Where(t => t.Type != TokenType.LineComment).GetSuperEnumerator();
            var body = GetBody(enumerator);
            Recipe = Expression.Lambda<Action<IConfigTransformer>>(body, Transformer);
            Log.Debug(string.Join(Environment.NewLine, "Recipe:", Recipe.ToCSharp()));
        }

        private static Expression GetBody(ISuperEnumerator<Token> enumerator)
        {
            return !enumerator.MoveNext() ? Expression.Empty() : new BlockParser(enumerator).Result;
        }
    }
}
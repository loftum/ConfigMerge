using System;
using System.Linq.Expressions;
using System.Reflection;
using ConfigMerge.Core.Formatting;
using NUnit.Framework;

namespace ConfigMerge.UnitTesting.Misc
{
    [TestFixture]
    public class ExpressionTest
    {
        private static MethodInfo ConsoleWriteLine => typeof(Console).GetMethod("WriteLine", new[] { typeof(string) });

        [Test]
        public void PrintVariable()
        {
            //string a;
            var a = Expression.Variable(typeof (string), "a");

            var block = Expression.Block(new[] {a},
                // a = "hei";
                Expression.Assign(a, Expression.Constant("hei")),
                // Console.WriteLine(a);
                Expression.Call(ConsoleWriteLine, a)
            );
            var lambda = Expression.Lambda(block);
            Console.WriteLine(lambda.ToCSharp());
            lambda.Compile().DynamicInvoke();
        }

        [Test]
        public void LambdaParameter()
        {
            var sayer = Expression.Variable(typeof (ISayer), "sayer");
            var block = Expression.Block(new ParameterExpression[0],
                Expression.IfThen(Expression.Equal(sayer, Expression.Constant(null)),
                    Expression.Assign(sayer, Expression.New(typeof(Sayer).GetConstructor(new Type[0])))),
                Expression.Call(ConsoleWriteLine, Expression.Call(sayer, typeof(ISayer).GetMethod("Something")))
                );
            
            var lambda = Expression.Lambda(block, sayer);
            
            Console.WriteLine(CSharpishFormatter.Format(lambda));

            lambda.Compile().DynamicInvoke(new Sayer());
        }
    }

    public interface ISayer
    {
        string Something();
    }

    public class Sayer : ISayer
    {
        public string Something()
        {
            return "Hey";
        }
    }
}
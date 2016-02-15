using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace ConfigMerge.UnitTesting.Misc
{
    [TestFixture]
    public class ReflectionTest
    {
        [Test]
        public void Should()
        {
            foreach (var method in typeof(string).GetMethods().Where(m => m.Name == "Concat"))
            {
                Console.WriteLine(Format(method));
            }
        }

        private string Format(MethodInfo method)
        {
            return $"{method.ReturnType} {method.Name}({string.Join(", ", method.GetParameters().Select(Format))})";
        }

        private string Format(ParameterInfo parameter)
        {
            return $"{parameter.ParameterType.Name} {parameter.Name}";
        }
    }
}
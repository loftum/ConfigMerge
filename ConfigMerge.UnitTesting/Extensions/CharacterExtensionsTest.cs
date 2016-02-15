using ConfigMerge.Core.Lang;
using NUnit.Framework;

namespace ConfigMerge.UnitTesting.Extensions
{
    [TestFixture]
    public class CharacterExtensionsTest
    {
        [Test]
        public void Symbols()
        {
            var symbols = new[] {'=', '$'};
            foreach (var symbol in symbols)
            {
                Assert.That(symbol.IsSymbol(), $"{symbol} is not a symbol");
            }
        }

        [Test]
        public void Punctuations()
        {
            var punctuations = new[] {'.', '[', ']', ';' , ',', '"', '\''};
            foreach (var punctuation in punctuations)
            {
                Assert.That(punctuation.IsPunctuation(), $"{punctuation} is not a punctuation");
            }
        }

        [Test]
        public void Letters()
        {
            const string letters = "abcdefghijklmnopqrstuvwxyzæøåABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅ";
            foreach (var letter in letters)
            {
                Assert.That(letter.IsLetter(), $"{letter} is not a letter");
            }
        }

        [Test]
        public void Numbers()
        {
            const string numbers = "1234567890";
            foreach (var number in numbers)
            {
                Assert.That(number.IsNumber(), $"{number} is not a number");
            }
        }
    }
}
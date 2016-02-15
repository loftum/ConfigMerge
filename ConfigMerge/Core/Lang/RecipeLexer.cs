using System.Collections;
using System.Text;
using ConfigMerge.Collections;
using ConfigMerge.Logging;

namespace ConfigMerge.Core.Lang
{
    public class RecipeLexer : ISuperEnumerator<Token>
    {
        private static readonly ILogger Log = Logger.For<RecipeLexer>();

        private readonly SourceStreamEnumerator _enumerator;

        public RecipeLexer(SourceStreamEnumerator enumerator)
        {
            _enumerator = enumerator;
            _enumerator.MoveNext();
        }

        private Token _current;
        
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            IsDisposed = true;
            _enumerator.Dispose();
        }

        public void Reset()
        {
            _enumerator.Reset();
            Current = default(Token);
        }

        object IEnumerator.Current => Current;


        public Token Current
        {
            get { return _current; }
            private set
            {
                _current = value;
                Log.Trace(_current);
            }
        }

        public bool Moved { get; private set; }

        public bool MoveNext()
        {
            Moved = DoMoveNext();
            return Moved;
        }

        private bool DoMoveNext()
        {
            while (_enumerator.Moved)
            {
                var current = _enumerator.Current;
                if (current == '#')
                {
                    Current = ReadLineComment();
                    return true;
                }
                if (current == '"')
                {
                    Current = ReadString();
                    return true;
                }
                if (current.IsLetter())
                {
                    Current = ReadAlphanumeric();
                    return true;
                }
                if (current.IsWhitespace())
                {
                    Current = ReadWhitespace();
                    return true;
                }
                if (current.IsNumber())
                {
                    Current = ReadNumber();
                    return true;
                }
                if (current.IsSymbol())
                {
                    Current = ReadSymbol();
                    return true;
                }
                if (current.IsPunctuation())
                {
                    Current = ReadPunctuation();
                    return true;
                }
                throw new RecipeCompilerException(Current.Position, $"Invalid character '{current}'");
            }
            return false;
        }

        private Token ReadLineComment()
        {
            var value = new StringBuilder();
            var position = _enumerator.Position;
            while (_enumerator.Current != '\n')
            {
                value.Append(_enumerator.Current);
                if (!_enumerator.MoveNext())
                {
                    break;
                }
            }
            return new Token(TokenType.LineComment, position, value.ToString());
        }

        private Token ReadWhitespace()
        {
            var value = new StringBuilder();
            var position = _enumerator.Position;
            while (_enumerator.Current.IsWhitespace())
            {
                value.Append(_enumerator.Current);
                if (!_enumerator.MoveNext())
                {
                    break;
                }
            }
            return new Token(TokenType.Whitespace, position, value.ToString());
        }

        private Token ReadSymbol()
        {
            var token = new Token(TokenType.Symbol, _enumerator.Position, _enumerator.Current.ToString());
            _enumerator.MoveNext();
            return token;
        }

        private Token ReadNumber()
        {
            var value = new StringBuilder();
            var position = _enumerator.Position;
            while (_enumerator.Current.IsNumber())
            {
                value.Append(_enumerator.Current);
                if (!_enumerator.MoveNext())
                {
                    break;
                }
            }
            return new Token(TokenType.Number, position, value.ToString());
        }

        private Token ReadPunctuation()
        {
            var token = new Token(TokenType.Punctuation, _enumerator.Position, _enumerator.Current.ToString());
            _enumerator.MoveNext();
            return token;
        }

        private Token ReadString()
        {
            var start = _enumerator.Position;
            if (!_enumerator.MoveNext())
            {
                throw new RecipeCompilerException(start, "Neverending string");
            }
            var value = new StringBuilder();
            var position = _enumerator.Position;
            while (_enumerator.Current != '"')
            {
                value.Append(_enumerator.Current);
                if (!_enumerator.MoveNext())
                {
                    throw new RecipeCompilerException(start, "Neverending string");
                }
            }
            _enumerator.MoveNext();
            return new Token(TokenType.String, position, value.ToString());
        }

        private Token ReadAlphanumeric()
        {
            var value = new StringBuilder();
            var position = _enumerator.Position;
            while (_enumerator.Current.IsLetterOrDigit())
            {
                value.Append(_enumerator.Current);
                if (!_enumerator.MoveNext())
                {
                    break;
                }
            }
            return new Token(TokenType.String, position, value.ToString());
        }
    }
}
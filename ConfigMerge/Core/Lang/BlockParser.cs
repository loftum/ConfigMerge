using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using ConfigMerge.Collections;
using ConfigMerge.Common;
using ConfigMerge.Core.Formatting;
using ConfigMerge.Logging;

namespace ConfigMerge.Core.Lang
{
    public class BlockParser
    {
        private static readonly ILogger Log = Logger.For<BlockParser>();
        private readonly IDictionary<string, ParameterExpression> _variables = new Dictionary<string, ParameterExpression>();
        private readonly IList<Expression> _expressions = new List<Expression>();

        private readonly ISuperEnumerator<Token> _enumerator;

        public BlockParser(ISuperEnumerator<Token> enumerator)
        {
            _enumerator = enumerator;
            Result = Parse();
        }

        public Expression Result { get; }

        private Expression Parse()
        {
            foreach (var expression in DoParse())
            {
                Log.Trace($"Parsed {expression.ToCSharp()}");
                _expressions.Add(expression);
            }
            var block = Expression.Block(_variables.Values, _expressions);
            Log.Trace($"Block: {block.ToCSharp()}");
            return block;
        }

        private IEnumerable<Expression> DoParse()
        {
            AdvanceWhile(c => c.Current.Type == TokenType.Whitespace, false);
            while (_enumerator.Moved)
            {
                yield return ParseStatement();
                AdvanceWhile(c => c.Current.Type == TokenType.Whitespace, false);
            }
        }

        private void Advance(bool throwOnEnd = true)
        {
            var last = _enumerator.Current;
            if (!_enumerator.MoveNext() && throwOnEnd)
            {
                throw RecipeCompilerException.UnexpectedEndAt(last);
            }
        }

        private void AdvanceWhile(Func<IEnumerator<Token>, bool> condition, bool throwOnEnd = true)
        {
            while (condition(_enumerator))
            {
                Advance(throwOnEnd);
            }
        }

        private Expression ParseStatement()
        {
            if (_enumerator.Current.Value == "var")
            {
                return AssignVariable();
            }
            var output = ParseLeftSide();
            if (_enumerator.Current.Value != "=")
            {
                throw new RecipeCompilerException(_enumerator.Current.Position, $"Expected '=' but got '{_enumerator.Current.Value}'");
            }
            Advance();
            AdvanceWhile(e => e.Current.Type == TokenType.Whitespace);
            var input = ParseInput();

            var arguments = new List<Expression>
            {
                output,
                input
            };
            AdvanceWhile(e => e.Current.Value == ";", false);
            return Expression.Call(RecipeParser.Transformer, RecipeParser.Transform, arguments);
        }

        private Expression ParseInput()
        {
            var input = ParseRightSide().StripQuotes();
            var array = input as NewArrayExpression;
            if (array != null)
            {
                return array;
            }
            return Expression.NewArrayInit(input.Type, input);
        }

        private BinaryExpression AssignVariable()
        {
            Expect("var");
            Advance();
            AdvanceWhile(e => e.Current.Type == TokenType.Whitespace);
            var token = _enumerator.Current;
            var variableName = ReadUntil(e => e.Current.Type.In(TokenType.Whitespace, TokenType.Symbol));
            if (_variables.ContainsKey(variableName))
            {
                throw new RecipeCompilerException(token.Position, $"Variable {variableName} already defined");
            }
            AdvanceWhile(e => e.Current.Type == TokenType.Whitespace);
            Expect("=");
            Advance();
            AdvanceWhile(e => e.Current.Type == TokenType.Whitespace);
            var rightSide = ParseRightSide();

            var variable = Expression.Variable(rightSide.Type, variableName);
            _variables[variable.Name] = variable;
            Advance(false);
            return Expression.Assign(variable, rightSide);
        }

        private void Expect(string value)
        {
            if (_enumerator.Current.Value != value)
            {
                throw new RecipeCompilerException(_enumerator.Current.Position, $"Invalid token '{_enumerator.Current.Value}'. Expected '{value}'");
            }
        }

        private Expression ParseLeftSide()
        {
            if (!_enumerator.Current.LooksLikeFilePath())
            {
                throw new RecipeCompilerException(_enumerator.Current.Position, $"Output cannot start with '{_enumerator.Current.Value}'");
            }

            var value = ReadWhile(e => e.Current.Type.In(TokenType.String) || e.Current.Value == ".");

            var left = _variables.ContainsKey(value) ? (Expression)_variables[value] : Expression.Constant(value);
            AdvanceWhile(e => e.Current.Type == TokenType.Whitespace);
            if (_enumerator.Current.Value == "+")
            {
                Advance();
                AdvanceWhile(e => e.Current.Type == TokenType.Whitespace);
                return CustomExpression.Concat(left, ParseLeftSide());
            }
            return left;
        }

        private Expression ParseRightSide()
        {
            var left = ParseExpression();
            AdvanceWhile(e => e.Current.Type == TokenType.Whitespace);
            if (!_enumerator.Moved || _enumerator.Current.Value.In(",", "]", ";", ")"))
            {
                return left;
            }
            if (_enumerator.Current.Value == "+")
            {
                Advance();
                AdvanceWhile(e => e.Current.Type == TokenType.Whitespace);
                var right = ParseRightSide();
                var result = CustomExpression.Concat(left, right);
                return result;
            }
            throw new RecipeCompilerException(_enumerator.Current.Position, $"Unexpected token '{_enumerator.Current.Value}'");
        }

        private Expression ParseExpression()
        {
            if (_enumerator.Current.Value == "[")
            {
                return Expression.NewArrayInit(typeof(string), ParseArrayValues());
            }
            if (_enumerator.Current.Value == "(")
            {
                var operand = ParseUnaryOperand();
                return Expression.MakeUnary(ExpressionType.Convert, operand, operand.Type);
            }

            var value = ReadWhile(e => e.Current.LooksLikeFilePath());
            return _variables.ContainsKey(value) ? (Expression)_variables[value] : Expression.Constant(value);
        }

        private Expression ParseUnaryOperand()
        {
            Expect("(");
            Advance();
            AdvanceWhile(e => e.Current.Type == TokenType.Whitespace);
            var operand = ParseRightSide();
            Expect(")");
            Advance(false);
            return operand;
        }

        private IEnumerable<Expression> ParseArrayValues()
        {
            Expect("[");
            Advance();
            AdvanceWhile(e => e.Current.Type == TokenType.Whitespace);

            while (_enumerator.Current.Value != "]")
            {
                AdvanceWhile(e => e.Current.Type == TokenType.Whitespace);
                yield return ParseRightSide();
                
                if (_enumerator.Current.Value != "]")
                {
                    Advance();
                }
            }
            Advance(false);
        }

        private string ReadWhile(Func<IEnumerator<Token>, bool> condition, bool throwOnEnd = false)
        {
            var value = new StringBuilder();
            while (condition(_enumerator))
            {
                value.Append(_enumerator.Current.Value);
                if (!_enumerator.MoveNext())
                {
                    if (throwOnEnd)
                    {
                        throw RecipeCompilerException.UnexpectedEndAt(_enumerator.Current);
                    }
                    break;
                }
            }
            return value.ToString();
        }

        private string ReadUntil(Func<IEnumerator<Token>, bool> condition, bool throwOnEnd = false)
        {
            var value = new StringBuilder();
            while (!condition(_enumerator))
            {
                value.Append(_enumerator.Current.Value);
                if (!_enumerator.MoveNext())
                {
                    if (throwOnEnd)
                    {
                        throw RecipeCompilerException.UnexpectedEndAt(_enumerator.Current);
                    }
                    break;
                }
            }
            return value.ToString();
        }
    }
}
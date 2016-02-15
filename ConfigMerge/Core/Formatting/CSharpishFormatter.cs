using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ConfigMerge.Common;

namespace ConfigMerge.Core.Formatting
{
    public class CSharpishFormatter : ExpressionVisitor
    {
        private int _level;
        public string Formatted { get; }

        public CSharpishFormatter(Expression expression)
        {
            Formatted = Printable(Visit(expression));
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var builder = new StringBuilder();
            switch (node.Parameters.Count)
            {
                case 0:
                    builder.Append("() => ");
                    break;
                case 1:
                    builder.AppendFormat("{0} => ", node.Parameters.Single().Name);
                    break;
                default:
                    builder.AppendFormat("({0}) => ", string.Join(", ", node.Parameters.Select(p => p.Name)));
                    break;
            }
            if (node.Body is BlockExpression)
            {
                builder.AppendLine();
            }
            builder.Append(Printable(Visit(node.Body)));
            return Expression.Constant(builder.ToString());
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            var builder = new StringBuilder()
                .AppendFormat("if ({0})", Printable(Visit(node.Test)))
                .AppendLine();
            _level++;
            
            using (new Block(builder))
            {
                var indent = CurrentIndent;
                var lines = Printable(Visit(node.IfTrue)).SplitLines();
                foreach (var line in lines)
                {
                    builder.Append(indent).AppendLine(line);
                }
            }
            _level--;
            return Expression.Constant(builder.ToString());
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var builder = new StringBuilder()
                .Append(Printable(Visit(node.Left)))
                .AppendFormat(" {0} ", Map(node.NodeType))
                .Append(Printable(Visit(node.Right)));
            return Expression.Constant(builder.ToString());
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            return Visit(node.Operand);
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            _level++;
            var indent = CurrentIndent;
            var builder = new StringBuilder();
            using (new Block(builder))
            {
                foreach (var variable in node.Variables)
                {
                    builder.Append(indent).AppendFormat("{0} {1};", variable.Type.Name, variable.Name).AppendLine();
                }
                foreach (var expression in node.Expressions)
                {
                    var lines = Printable(Visit(expression)).SplitLines();
                    foreach (var line in lines)
                    {
                        builder.Append(indent).Append(line).AppendLine();
                    }
                }
            }
            _level--;
            return Expression.Constant(builder.ToString());
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return node;
        }

        private static string Map(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.Assign:
                    return "=";
                case ExpressionType.Add:
                    return "+";
                case ExpressionType.Subtract:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Equal:
                    return "==";
            }
            return nodeType.ToString();
        }

        private string CurrentIndent => Indent(_level);

        public static string Indent(int level)
        {
            return string.Join("", Enumerable.Range(0, level).Select(i => "  "));
        }

        private static string Printable(Expression expression)
        {
            var constant = expression as ConstantExpression;
            if (constant != null)
            {
                return ToString(constant.Value);
            }
            
            return expression.ToString();
        }

        private static string ToString(object value)
        {
            var expression = value as Expression;
            if (expression != null)
            {
                return Printable(expression);
            }
            if (value == null)
            {
                return "null";
            }
            var s = value as string;
            if (s != null)
            {
                return s;
            }
            var type = value.GetType();
            if (type.IsValueType)
            {
                return value.ToString();
            }
            var collection = value as ICollection;
            if (collection != null)
            {
                var values = collection.Cast<object>().Select(ToString);
                return $"[{string.Join(", ", values)}]";
            }
            return value.ToString();
        }

        public override string ToString()
        {
            return Formatted;
        }

        public static string Format(Expression expression)
        {
            return new CSharpishFormatter(expression).Formatted;
        }
    }

    public class Block : IDisposable
    {
        private readonly StringBuilder _builder;

        public Block(StringBuilder builder)
        {
            _builder = builder;
            _builder.AppendLine("{");
        }

        public void Dispose()
        {
            _builder.AppendLine("}");
        }
    }

    public static class CSharpFormatterExtensions
    {
        public static string ToCSharp(this Expression expression)
        {
            return CSharpishFormatter.Format(expression);
        }
    }
}
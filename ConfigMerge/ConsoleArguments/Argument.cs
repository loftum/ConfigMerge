using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigMerge.ConsoleArguments
{
    public class Argument
    {
        public Type Type { get; set; }
        public bool IsFlag => Type == typeof (bool);
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }

        private IEnumerable<object> GetPossibleValues()
        {
            return Type.IsEnum
                ? Enum.GetValues(Type).Cast<object>().Select(o => o.ToString().ToLowerInvariant())
                : Enumerable.Empty<object>();
        }

        public override string ToString()
        {
            var builder = new StringBuilder().Append($"-{Name}");
            var description = Description ?? "value";
            if (IsFlag)
            {
                builder.Append($" ({description})");
            }
            else
            {
                var values = string.Join(", ", GetPossibleValues());

                builder.Append($":<{description}");
                if (values.Any())
                {
                    builder.Append($"; {string.Join(", ", values)}");
                }
                builder.Append(">");
            }
            return ToString(builder.ToString());
        }

        private string ToString(string value)
        {
            return Required ? value : $"[{value}]";
        }
    }
}
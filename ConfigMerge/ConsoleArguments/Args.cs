using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConfigMerge.ConsoleArguments
{
    public static class Args
    {
        private static IEnumerable<Argument> GetArguments(this Type type)
        {
            return from p in type.GetProperties()
                let a = p.GetCustomAttribute<ArgumentAttribute>()
                orderby a != null
                let required = a?.Required ?? false
                let description = a?.Description
                select new Argument
                {
                    Type = p.PropertyType,
                    Name = p.Name.ToLowerInvariant(),
                    Description = a?.Description,
                    Required = a?.Required ?? false
                };
        }

        public static string GetUsage<T>()
        {
            var type = typeof (T);
            return string.Join(" ", type.GetArguments());
        }

        public static T To<T>(this string[] args) where T : class, new()
        {
            var type = typeof (T);
            var properties = type.GetProperties();

            var t = new T();
            foreach (var arg in args)
            {
                if (arg.Contains(":"))
                {
                    var parts = arg.Split(':');
                    if (parts.Length < 2)
                    {
                        throw new ArgumentException($"Bad argument {arg} {parts.Length}");
                    }
                    var name = parts[0].TrimStart('-');
                    var property =
                        properties.FirstOrDefault(
                            p => string.Equals(p.Name, name, StringComparison.CurrentCultureIgnoreCase));
                    if (property == null)
                    {
                        throw new ArgumentException($"Unknown option -{name}");
                    }

                    var inputValue = string.Join(":", parts.Skip(1));
                    var value = property.PropertyType.IsEnum
                        ? Enum.Parse(property.PropertyType, inputValue, true)
                        : Convert.ChangeType(inputValue, property.PropertyType);
                    property.SetValue(t, value);
                }
                else
                {
                    var name = arg.TrimStart('-');
                    var property =
                        properties.FirstOrDefault(
                            p => string.Equals(p.Name, name, StringComparison.CurrentCultureIgnoreCase));
                    if (property == null)
                    {
                        throw new ArgumentException($"Bad flag {arg}");
                    }
                    if (property.PropertyType != typeof (bool))
                    {
                        throw new ArgumentException($"Invalid flag {arg}");
                    }
                    property.SetValue(true, t);
                }
            }
            return t;
        }

        public static bool NeedsHelp(this string[] args)
        {
            return args == null ||
                   !args.Any() ||
                   args.Contains("--help") ||
                   args.Contains("?");
        }
    }
}
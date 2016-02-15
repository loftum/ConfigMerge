using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConfigMerge.Common
{
    public static class FriendlyExtensions
    {
        public static string FriendlyCommaSeparated<T>(this IEnumerable<T> items, string lastSeparator)
        {
            if (items == null)
            {
                return string.Empty;
            }
            var collection = items.ToList();
            if (!collection.Any())
            {
                return string.Empty;
            }
            if (collection.Count == 1)
            {
                return collection.Single().ToString();
            }
            if (collection.Count == 2)
            {
                return string.Join($" {lastSeparator} ", collection);
            }
            var commaSeparatedFirst = string.Join(", ", collection.Take(collection.Count - 1));
            var last = collection.Last();
            return string.Join($" {lastSeparator} ", commaSeparatedFirst, last);
        }

        public static string Friendly(this object item)
        {
            if (item == null)
            {
                return "null";
            }
            var collection = item as IList;
            if (collection != null)
            {
                return $"[{string.Join(", ", collection.Cast<object>().Select(o => o.Friendly()))}]";
            }
            return item.ToString();
        }
    }
}
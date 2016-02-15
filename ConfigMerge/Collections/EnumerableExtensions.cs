using System.Collections.Generic;

namespace ConfigMerge.Collections
{
    public static class EnumerableExtensions
    {
        public static ISuperEnumerator<T> GetSuperEnumerator<T>(this IEnumerable<T> enumerable)
        {
            var enumerator = enumerable.GetEnumerator();
            var super = enumerator as ISuperEnumerator<T>;
            return super ?? new SuperEnumerator<T>(enumerator);
        }
    }
}
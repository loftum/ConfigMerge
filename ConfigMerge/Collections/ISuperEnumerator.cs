using System.Collections.Generic;

namespace ConfigMerge.Collections
{
    public interface ISuperEnumerator<out T> : IEnumerator<T>
    {
        bool Moved { get; }
    }
}
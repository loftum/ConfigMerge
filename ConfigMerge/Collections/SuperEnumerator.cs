using System.Collections;
using System.Collections.Generic;

namespace ConfigMerge.Collections
{
    public class SuperEnumerator<T> : ISuperEnumerator<T>
    {
        private readonly IEnumerator<T> _target;

        public SuperEnumerator(IEnumerator<T> target)
        {
            _target = target;
        }

        public void Dispose()
        {
            _target.Dispose();
        }

        public bool Moved { get; private set; }

        public bool MoveNext()
        {
            Moved = _target.MoveNext();
            return Moved;
        }

        public void Reset()
        {
            _target.Reset();
            Moved = false;
        }

        public T Current => _target.Current;

        object IEnumerator.Current => Current;
    }
}
using System;
using System.Collections;
using System.IO;
using ConfigMerge.Collections;

namespace ConfigMerge.Core.Lang
{
    public class SourceStreamEnumerator : ISuperEnumerator<char>
    {
        public bool IsDisposed { get; private set; }

        private StreamReader _reader;

        private readonly IRecipeSource _source;
        private int _lineNumber;
        private int _columnNumber;

        public SourcePosition Position => new SourcePosition(_source, _lineNumber, _columnNumber);

        public SourceStreamEnumerator(IRecipeSource source)
        {
            _source = source;
            Init();
        }

        public bool Moved { get; private set; }

        private bool DoMoveNext()
        {
            if (_reader.EndOfStream)
            {
                return false;
            }
            var read = _reader.Read();
            if (read == -1)
            {
                return false;
            }
            var lineFeed = _lineNumber == 0 || Current == '\n';
            if (lineFeed)
            {
                _lineNumber++;
                _columnNumber = 1;
            }
            else
            {
                _columnNumber++;
            }
            Current = (char) read;
            return true;
        }

        public bool MoveNext()
        {
            Moved = DoMoveNext();
            return Moved;
        }

        public void Reset()
        {
            _reader.Dispose();
            Init();
        }

        private void Init()
        {
            _lineNumber = 0;
            _columnNumber = 0;
            _reader = new StreamReader(_source.GetStream());
        }

        public char Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            IsDisposed = true;
            _reader.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
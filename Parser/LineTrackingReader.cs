#nullable disable

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Parsing.Junos
{
    /// <summary>
    /// A wrapper for a TextReader that keeps track line and column as well as allowing
    /// you to unread text using a PushBack method.
    /// </summary>
    public class LineTrackingReader
    {
        private readonly TextReader _reader;
        private List<char> _buffer = new List<char>();

        /// <summary>
        /// The, currently read to, Line in our reader.
        /// </summary>
        public int Line { get; private set; } = 0;

        /// <summary>
        /// The, currently read to, column in our reader.
        /// </summary>
        /// <value>The column.</value>
        public int Column { get; private set; } = 0;

        private Stack<int> _lineLengths = new Stack<int>();

        public LineTrackingReader(TextReader reader)
        {
            _reader = reader;
        }

        public int Read()
        {
            int c;
            if (_buffer.Count > 0)
            {
                c = _buffer.First();
                _buffer = new List<char>(_buffer.Skip(1));
            }
            else
            {
                c = _reader.Read();
            }
            if (c != -1)
            {
                updateLineAndColumn(new[] { (char)c }, false);
            }
            return c;
        }

        /// <summary>
        /// Read from the wrapped reader into the provided buffer while updating the line
        /// and column.
        /// </summary>
        /// <param name="buffer">The char buffer to read text into.
        /// Overwrites the contents if there was any.</param>
        public int Read(char[] buffer)
        {
            var n = 0;
            if (_buffer.Count > 0)
            {
                var delta = _buffer.Count - buffer.Length;
                if (delta <= 0)
                { // all of the internal buffer will fit in our output buffer.
                    _buffer.CopyTo(buffer);
                    n = _buffer.Count;
                    _buffer.Clear();
                }
                else
                { // only part of the _buffer will fit in our output buffer
                    _buffer.CopyTo(0, buffer, 0, delta);
                    _buffer = new List<char>(_buffer.Skip(delta));
                    n = delta;
                }
            }
            if (n < buffer.Length)
            {
                var nn = _reader.Read(buffer, n, buffer.Length - n);
                n += nn;
            }
            if (n != 0)
                updateLineAndColumn(buffer.Take(n).ToArray(), false);
            // track lines and columns
            return n;
        }

        /// <summary>
        /// Pushes back text into the reader to be read again while updating the line and
        /// column numbers.
        /// This method assumes you are pushing previously read text back in.
        /// </summary>
        /// <param name="buffer">Buffer.</param>
        public void PushBack(char[] buffer)
        {
            updateLineAndColumn(buffer, true);
            var tmp = new List<char>();
            tmp.AddRange(buffer);
            tmp.AddRange(_buffer);
            _buffer = tmp;
        }

        public void PushBack(char c)
        {
            PushBack(new[] { c });
        }

        private void decrementLine()
        {
            Line--;
            Column = _lineLengths.Pop();
        }

        private void incrementLine()
        {
            Line++;
            _lineLengths.Push(Column);
            Column = 0;
        }

        private void updateLineAndColumn(IEnumerable<char> buffer, bool reverse)
        {
            if (reverse)
            {
                foreach (var c in buffer.Reverse())
                {
                    switch (c)
                    {
                        case '\n':
                            decrementLine();
                            break;

                        default:
                            Column--;
                            break;
                    }
                }
            }
            else
            {
                foreach (var c in buffer)
                {
                    switch (c)
                    {
                        case '\n':
                            incrementLine();
                            break;

                        default:
                            Column++;
                            break;
                    }
                }
            }
        }
    }
}
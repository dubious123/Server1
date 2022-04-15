using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ServerCore
{
    public class RecvBuffer
    {
        byte[] _buffer;
        int _readPos = 0;
        int _writePos = 0;
        public RecvBuffer(ushort size)
        {
            _buffer = new byte[size];          
        }
        public ArraySegment<byte> ReadSegment
        {
            get { return new ArraySegment<byte>(_buffer, _readPos, _writePos - _readPos); }
        }
        public ArraySegment<byte> WriteSegment
        {
            get { return new ArraySegment<byte>(_buffer, _writePos, _buffer.Length - _writePos); }
        }
        public ArraySegment<byte> Read(int size)
        {
            //Todo
            var segment = ReadSegment;
            if (segment.Count < size)
                return null;
            var result = new ArraySegment<byte>(segment.Array, _readPos, size);
            Interlocked.Add(ref _readPos, size);
            return result;
        }
        public bool CanRead()
        {
            return _writePos - _readPos > 4;
        }
        public bool OnWrite(int size)
        {
            _writePos += size;
            return _writePos < _buffer.Length;
        }
        public void Clear()
        {
            var leftDataSize = _readPos - _writePos;
            if (leftDataSize != 0)
            {
                Array.Copy(_buffer, _readPos, _buffer, 0, leftDataSize);
                _writePos = leftDataSize;
                return;
            }
            _readPos = _writePos = 0;
        }
    }
}

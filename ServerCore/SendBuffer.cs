using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ServerCore
{
    public class SendBuffer
    {
        protected List<ArraySegment<byte>> _sendList = new List<ArraySegment<byte>>();
        object _lock = new object();
        public void Write(ArraySegment<byte> data)
        {
            _sendList.Add(data);
        }
        public List<ArraySegment<byte>> ReadAll()
        {
            var sendList = new List<ArraySegment<byte>>();
            lock (_lock)
            {
                foreach (var segment in _sendList)
                {
                    if (segment != null)
                        sendList.Add(segment);
                }
            }
            _sendList.Clear();
            return sendList;
        }
        public bool IsEmpty()
        {
            return _sendList.Count == 0;
        }
    }
}

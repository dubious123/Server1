using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class SendBuffer
    {
        protected List<ArraySegment<byte>> _sendList = new List<ArraySegment<byte>>();
        public void Write(ArraySegment<byte> data)
        {
            _sendList.Add(data);
        }
        public List<ArraySegment<byte>> ReadAll()
        {
            var sendList = new List<ArraySegment<byte>>();
            sendList.AddRange(_sendList);
            _sendList.Clear();
            return sendList;
        }
    }
}

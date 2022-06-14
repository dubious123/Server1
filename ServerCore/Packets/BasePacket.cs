using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class BasePacket
    {
        public ushort PacketId = 0;
        public uint PacketError;
        public string Message;
    }
}

using PacketTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class S_LoginPacket : IPacket
    {
        public bool Accepted;
        public S_LoginPacket(bool accept)
        {
            PacketId = (ushort)Define.PacketId.s_login;
            Accepted = accept;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketTools;

namespace ServerCore
{
    public class C_LoginPacket : IPacket
    {
        public readonly string ID;
        public readonly string PW;
        public C_LoginPacket(string id, string pw)
        {
            PacketId = (ushort)Define.PacketId.c_login;
            ID = id;
            PW = pw;
        }
    }
}

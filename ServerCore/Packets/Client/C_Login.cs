using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketTools;

namespace ServerCore
{
    public class C_Login : BasePacket
    {
        public readonly string ID;
        public readonly string PW;
        public C_Login(string id, string pw)
        {
            PacketId = (ushort)Define.P_Id.c_login;
            ID = id;
            PW = pw;
        }
    }
}

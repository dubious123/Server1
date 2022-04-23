using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketTools;

namespace ServerCore.Packets.Client
{
    public class C_Logout : IPacket
    {
        public UserInfo User;
        public C_Logout(UserInfo user)
        {
            PacketId = (ushort)Define.P_Id.c_logout;
            User = user;
        }
    }
}

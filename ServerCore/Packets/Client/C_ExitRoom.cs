using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketTools;

namespace ServerCore.Packets.Client
{
    public class C_ExitRoom : BasePacket
    {
        public UserInfo User;
        public C_ExitRoom(UserInfo user)
        {
            PacketId = (ushort)Define.P_Id.c_exitRoom;
            User = user;
        }
    }
}

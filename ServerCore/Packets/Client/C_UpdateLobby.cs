using PacketTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore.Packets
{
    public class C_UpdateLobby : IPacket
    {
        public UserInfo User;
        public C_UpdateLobby(UserInfo user)
        {
            PacketId = (ushort)Define.P_Id.c_updateLobby;
            User = user;
        }
    }
}

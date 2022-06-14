using PacketTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class C_EnterLobby : BasePacket
    {
        public UserInfo User;
        public C_EnterLobby(UserInfo user)
        {
            PacketId = (ushort)Define.P_Id.c_enterLobby;
            User = user;
        }
    }
}

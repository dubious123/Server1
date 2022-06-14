using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketTools;

namespace ServerCore
{
    public class S_EnterLobby : BasePacket
    {
        public bool Accept;
        public UserInfo User;
        public S_EnterLobby(bool accept, UserInfo user)
        {
            PacketId = (ushort)Define.P_Id.s_enterLobby;
            Accept = accept;
            User = user;
        }
    }
}

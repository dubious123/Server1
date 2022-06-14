using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketTools;

namespace ServerCore.Packets.Server
{
    public class S_ExitRoom : BasePacket
    {
        public bool Answer;
        public UserInfo User;
        public S_ExitRoom(UserInfo user, bool answer)
        {
            PacketId = (ushort)Define.P_Id.s_exitRoom;
            User = user;
            Answer = answer;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketTools;

namespace ServerCore.Packets
{
    public class S_EnterRoom : BasePacket
    {
        public bool Answer;
        public UserInfo User;
        public S_EnterRoom()
        {
            PacketId = (ushort)Define.P_Id.s_enterRoom;
        }
    }
}

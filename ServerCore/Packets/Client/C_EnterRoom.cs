using PacketTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore.Packets
{
    public class C_EnterRoom : IPacket
    {
        public UserInfo User;
        public uint RoomId;
        public C_EnterRoom(UserInfo user, uint id)
        {
            PacketId = (ushort)Define.P_Id.c_enterRoom;
            User = user;
            RoomId = id;
        }
    }
}

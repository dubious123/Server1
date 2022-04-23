using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketTools;

namespace ServerCore.Packets 
{ 
    public class C_UpdateRoom : IPacket
    {
        public UserInfo User;
        public uint RoomId;
        public C_UpdateRoom(UserInfo user)
        {
            PacketId = (ushort)Define.P_Id.c_updateRoom;
            User = user;
            RoomId = user.RoomId;
        }
    }
}

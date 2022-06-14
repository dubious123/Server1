using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketTools;

namespace ServerCore.Packets
{
    public class S_UpdateRoom : BasePacket
    {
        public Chatting[] Chats;
        public S_UpdateRoom(Chatting[] chats)
        {
            PacketId = (ushort)Define.P_Id.s_updateRoom;
            Chats = chats;
        }
    }
}

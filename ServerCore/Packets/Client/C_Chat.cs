using PacketTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore.Packets.Client
{
    public class C_Chat : IPacket
    {
        public Chatting Chat;
        public C_Chat(Chatting chat)
        {
            PacketId = (ushort)Define.P_Id.c_chat;
            Chat = chat;
        }
    }
}

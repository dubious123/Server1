using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketTools
{
    public class TestPacket1 : IPacket
    {
        public string Name;
        public string Chat;
        public TestPacket1()
        {

        }
        public TestPacket1(ushort packetId, string name, string chat)
        {
            this.PacketId = packetId;
            this.Name = name;
            this.Chat = chat;
        }
    }
}

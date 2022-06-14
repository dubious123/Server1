using PacketTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore.Packets
{
    public class S_UpdateLobby :BasePacket
    {
        public LobbyInfo Info;
        public S_UpdateLobby(LobbyInfo info)
        {
            PacketId = (ushort)Define.P_Id.s_updateLobby;
            Info = info;
        }
    }
}

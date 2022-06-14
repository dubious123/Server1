using PacketTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore.Packets.Server
{
    public class S_Logout : BasePacket
    {
        public bool Answer;
        public S_Logout(bool answer, string message = null)
        {
            PacketId = (ushort)Define.P_Id.s_logout;
            Answer = answer;
            Message = message;
        }
    }
}

using PacketTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class S_Login : IPacket
    {
        public bool Accepted;
        public UserInfo User;
        public S_Login(bool accept, UserInfo user)
        {
            PacketId = (ushort)Define.P_Id.s_login;
            Accepted = accept;
            User = user;
        }
    }
}

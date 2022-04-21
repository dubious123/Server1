using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public static class Define
    {
        public enum Cmd_State
        {
            start,
            wait,
            done,
            error,
        }
        public enum PacketId
        {
            c_login,
            s_login,
        }
    }
}

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
        public enum P_Id
        {
            c_login,
            c_logout,
            c_enterLobby,
            c_updateLobby,
            c_enterRoom,
            c_exitRoom,
            c_updateRoom,
            c_chat,
            
            s_login,
            s_logout,
            s_enterLobby,
            s_updateLobby,
            s_enterRoom,
            s_exitRoom,
            s_updateRoom,
        }
        public enum User_State
        {
            logout,
            login,
            lobby,
            room
        }
    }
}

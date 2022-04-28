using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Chatting
    {
        public UserInfo User;
        public string Chat;
        public DateTime Time;
        public Chatting(UserInfo user, string chat)
        {
            User = user;
            Chat = chat;
            Time = DateTime.Now;
        }
    }
}

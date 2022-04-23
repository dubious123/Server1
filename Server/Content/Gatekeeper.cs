using Server.Data;
using ServerCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class InOutLog
    {
        public InOutLog(UserInfo user, bool enter, bool success)
        {
            Time = Environment.TickCount;
            User = user;
            Enter = enter;
            Result = success;
        }
        public UserInfo User;
        public int Time;
        public bool Enter;
        public bool Result;
    }
    public class Gatekeeper
    {
        static Gatekeeper _inst = new Gatekeeper();
        public static Gatekeeper Inst { get { return _inst; } }
        ConcurrentQueue<InOutLog> _inoutLogQueue = new ConcurrentQueue<InOutLog>();
        ConcurrentDictionary<string, UserInfo> _guestBook = new ConcurrentDictionary<string, UserInfo>();
        public UserInfo TryLogin(string id, string pw)
        {
            if (DataManager.Inst.GetUserData(id, out UserData user))
                if (user.Pw == pw)
                {
                    var info = new UserInfo(user);
                    info.UserState = Define.User_State.login;
                    InOutLog login = new InOutLog(info, true, _guestBook.TryAdd(info.Id,info));
                    _inoutLogQueue.Enqueue(login);
                    return info;
                }                  
            return null;         
        }
        public bool TryLogout(string id)
        {
            
            if(_guestBook.TryRemove(id, out var user) == false)
                return false;
            var inoutLog = new InOutLog(user, false, true);
            if (Concierge.Inst.TryCheckOut(user) == false)
                return false;

            _inoutLogQueue.Enqueue(inoutLog);
            return true;
        }
        public UserInfo EnterLobby(string id)
        {
            if (_guestBook.TryGetValue(id, out var user) == false)
                return null;
            if(Concierge.Inst.TryCheckIn(user) == false)
                return null;
            return user;
        }
    }
}

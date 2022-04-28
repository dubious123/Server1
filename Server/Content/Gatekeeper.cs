using Server.Data;
using ServerCore;
using ServerCore.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Server
{
    public class Gatekeeper
    {
        static Gatekeeper _inst = new Gatekeeper();
        public static Gatekeeper Inst { get { return _inst; } }
        ConcurrentDictionary<string, UserInfo> _guestBook = new ConcurrentDictionary<string, UserInfo>();
        TraceSource _ts;
        Gatekeeper()
        {
            _ts = LogMgr.GetTraceSource("Server");
        }

        public UserInfo TryLogin(string id, string pw)
        {
            _ts.TraceInfo($"[Gatekeeper] User[{id}] is trying to log-in");
            if (DataManager.Inst.GetUserData(id, out UserData user))
                if (user.Pw == pw)
                {
                    var info = new UserInfo(user);
                    info.UserState = Define.User_State.login;
                    _ts.TraceInfo($"[Gatekeeper] User[{id}] log-in success");
                    if(_guestBook.TryAdd(user.Id, info))
                        return info;
                    _ts.TraceEvent(TraceEventType.Warning, 20, $"[Gatekeeper] Adding User[{id}] to guest book failed");
                }
            _ts.TraceEvent(TraceEventType.Warning, 20, $"[Gatekeeper] User[{id}] log-in failed");
            return null;         
        }
        public bool TryLogout(string id)
        {
            _ts.TraceInfo($"[Gatekeeper] User[{id}] is trying to log-out");
            if (id == null)
            {
                _ts.TraceEvent(TraceEventType.Warning, 21, $"[Gatekeeper] log-out failed : id is null");
                return false;
            }
                

            if(_guestBook.TryRemove(id, out var user) == false)
            {
                _ts.TraceEvent(TraceEventType.Warning, 22, $"[Gatekeeper] log-out failed : removing user[{id}] from guestBook failed");
                return false;
            }                
            //if(user.UserState == Define.User_State.logout)
            if(user.UserState == Define.User_State.lobby || user.UserState == Define.User_State.room)
                if (Concierge.Inst.TryCheckOut(user) == false)
                    return false;
            return true;
        }
        public UserInfo EnterLobby(string id)
        {
            _ts.TraceInfo($"[Gatekeeper] user[{id}] is trying to enter lobby");
            if (_guestBook.TryGetValue(id, out var user) == false)
            {               
                _ts.TraceEvent(TraceEventType.Warning, 22, $"[Gatekeeper] EnterLobby failed : Finding user[{id}] from guestBook failed");
                return null;
            }
            if (Concierge.Inst.TryCheckIn(user) == false)
                return null;
            return user;
        }
    }
}

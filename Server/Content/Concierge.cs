using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ServerCore;
using Server.Data;
using System.Diagnostics;
using ServerCore.Log;

namespace Server
{
    public class Concierge
    {
        static Concierge _inst = new Concierge();
        public static Concierge Inst { get { return _inst; } }
        Lobby _lobby;
        ConcurrentDictionary<string, UserInfo> _checkin_dict;
        TraceSource _ts;
        TraceListener _tl;
        Concierge()
        {
            _lobby = DataManager.Inst.GetLobby();
            _checkin_dict = new ConcurrentDictionary<string, UserInfo>();

            _ts = LogMgr.GetTraceSource("Server");
            _tl = LogMgr.GetListener("Server", 1);
        }
        public LobbyInfo GetLobbyInfo()
        {
            return _lobby.GetInfo();
        }
        UserInfo GetUserById(string id)
        {
            if (_checkin_dict.TryGetValue(id, out var info) == false)
                _ts.TraceEvent(TraceEventType.Warning, 6, $"[concierge] Failed to get user[{id}] from check-in dict");
            return info;
        }
        public bool CheckUser(UserInfo user)
        {
            _ts.TraceInfo($"[concierge] checking user[{user.Id}]");
            if (_checkin_dict.TryGetValue(user.Id, out var original) == false)
            {
                _ts.TraceEvent(TraceEventType.Critical, 1, $"[concierge] cannot find user[{user.Id}] from check-in dict");
                return false;
            }
            if(original.Equals(user) == false)
            {
                _ts.TraceEvent(TraceEventType.Warning, 2, $"[concierge] user[{user.Id}] found from check-in dict is not the same user");
                return false;
            }
            return true;       
        }
        public bool TryCheckIn(UserInfo user)
        {
            _ts.TraceInfo($"[concierge] user[{user.Id}] is trying to check-in");
            if (_checkin_dict.ContainsKey(user.Id))
            {
                _ts.TraceEvent(TraceEventType.Critical, 3, $"[concierge] user[{user.Id}] is already in hotel but trying to check-in");
                return false;
            }
            user.UserState = Define.User_State.lobby;
            if(_checkin_dict.TryAdd(user.Id, user) == false)
            {
                _ts.TraceEvent(TraceEventType.Critical, 4, $"[concierge] failed to add user[{user.Id}] to check-in dict");
                return false;
            }
            _ts.TraceInfo($"[concierge] user[{user.Id}] check-in success");
            return true;
        }
        public bool TryCheckOut(UserInfo user)
        {
            _ts.TraceInfo($"[concierge] user[{user.Id}] trying to check-out");
            if (_checkin_dict.TryRemove(user.Id, out var guest) == false)
            {
                _ts.TraceEvent(TraceEventType.Critical, 5, $"[concierge] failed to remove user from check-in dict");
                return false;
            }
            if(user.UserState == Define.User_State.room)
            {
                _ts.TraceInfo($"[concierge] removing user[{user.Id}] from room {user.RoomId}");
                if (GetRoom(user.RoomId, out var room) == false)
                    return false;
                if (room.TryGetOut(user.Id) == false)
                {
                    _ts.TraceEvent(TraceEventType.Critical, 9, $"[concierge] User[{user.Id}] trying to get out from room {user.RoomId} but failed to find user in the room");
                    return false;
                }
            }
            user.UserState = Define.User_State.logout;
            user.RoomId = 0;
            return true;
        }
        public bool Check_ReserveRoom(Session session, UserInfo user, uint dest,ref string packetMessage)
        {
            _ts.TraceInfo($"[concierge] checking & Reserve room{dest} for user[{user.Id}]");
            if (user.UserState == Define.User_State.room)
            {
                _ts.TraceEvent(TraceEventType.Warning, 8, $"[concierge] User[{user.Id}] is in room already");
                packetMessage = "You are already in room";
                return false;
            }
            if (GetRoom(dest, out var room) == false)
            {
                packetMessage = "Invalid roomId";
                return false;
            }
                
            if (room.Info.CurrentUserNum >= room.Info.MaxUserNum)
            {
                _ts.TraceEvent(TraceEventType.Warning, 8, $"[concierge] failed to reserve : room {dest} is full from User[{user.Id}]");
                packetMessage = "Room is full";
                return false;
            }
            if (room.TryEnter(user.Id,session) == false)
            {
                _ts.TraceEvent(TraceEventType.Critical, 9, $"[concierge] User[{user.Id}] in session[{session.SessionID}] trying to re-enter same room");
                packetMessage = "found doppelganger";
                return false;
            }
            _ts.TraceInfo($"[concierge] checking & Reserve room{dest} for user[{user.Id}] success");
            return true;
        }
        public UserInfo GuideRoom(UserInfo user, uint dest)
        {
            _ts.TraceInfo($"[concierge] Guiding user[{user.Id}] to Room");
            var original = GetUserById(user.Id);
            if (original == null)
                return null;
            original.UserState = Define.User_State.room;
            original.RoomId = dest;
            return original;
        }
        public UserInfo GuideLobby(UserInfo user)
        {
            _ts.TraceInfo($"[concierge] Guiding user[{user.Id}] to Lobby");
            var original = GetUserById(user.Id);
            if (original == null)
                return null;
            if(original.UserState == Define.User_State.room)
            {
                if (GetRoom(user.RoomId, out var room) == false)
                    return null;
                if(room.TryGetOut(user.Id) == false)
                {
                    _ts.TraceEvent(TraceEventType.Critical, 11, $"[concierge] Failed to remove user[{user.Id}] from roomId[{user.RoomId}]");
                }
                
                
            }
            original.UserState = Define.User_State.lobby;
            original.RoomId = 0;
            return original;
        }
        public Chatting[] GetChatting(uint roomId)
        {
            _ts.TraceInfo($"getting chats from room[{roomId}]");
            if (GetRoom(roomId, out var room) == false)
                return null;

            return room.GetChats();
        }
        public Chatting[] PushChat(uint roomId, Chatting chat)
        {
            _ts.TraceInfo($"pushing chats to room[{roomId}]");
            if (GetRoom(roomId, out var room) == false)
                return null;
            room.PushChat(chat);
            return room.GetChats();
            
        }
        bool GetRoom(uint roomId, out Room room)
        {
            room = _lobby.GetRoom(roomId);
            if (room == null)
                _ts.TraceEvent(TraceEventType.Warning, 10, $"[concierge] Invalid roomId[{roomId}]");
            return room != null;
        }


    }
}

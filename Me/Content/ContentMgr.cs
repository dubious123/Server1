using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;
using ServerCore.Log;

namespace Me
{
    public class ContentMgr
    {
        static ContentMgr _inst = new ContentMgr();
        public static ContentMgr Inst { get { return _inst; } }

        public UserInfo User { get { return _user; } }
        UserInfo _user = new UserInfo() { UserState = Define.User_State.logout};

        LobbyInfo _lobby;
        Session _session;

        List<uint> _roomIdList;

        TraceSource _ts;
        ContentMgr()
        {
            _ts = LogMgr.GetTraceSource("Client");
        }
        public Session GetSession()
        {
            if(_session == null)
            {
                _session = SessionMgr.Inst.Find(1);
            }
            return _session;
        }
        public void UpdateUserInfo(UserInfo user)
        {
            _ts.TraceInfo("[contentMgr] updating user");
            _user = user;
        }
        public void UpdateLobby(LobbyInfo lobbyInfo)
        {
            _ts.TraceInfo("[contentMgr] updating lobby");
            _lobby = lobbyInfo;
            _roomIdList = new List<uint>();
            foreach (var roomInfo in _lobby.RoomInfoDict.Values)
                _roomIdList.Add(roomInfo.ID);
        }
        public void ShowLobby()
        {
            Console.WriteLine();
            for (int i = 0; i < _roomIdList.Count; i++)
            {
                _lobby.RoomInfoDict.TryGetValue(_roomIdList[i], out var roomInfo);
                Console.WriteLine($"Room {i} : {roomInfo.RoomName} {roomInfo.CurrentUserNum}/{roomInfo.MaxUserNum}");
            }
        }
        public void ShowChats(Chatting[] chats)
        {
            Console.WriteLine();
            foreach (var chat in chats)
            {
                if (chat == null)
                    break;
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine($"[{chat.Time}] [{chat.User.Name}] : {chat.Chat}");
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine();
            }
        }
        public bool IsValidRoomNum(int num)
        {
            var result = _roomIdList?.Count > num;
            if (result == false)
                _ts.TraceEvent(TraceEventType.Error, 1, "[ContentMgr] invalid roomNum");
            return result;
        }
        public RoomInfo GetRoomInfoByIndex(int index)
        {
            if(IsValidRoomNum(index))
                return _lobby.RoomInfoDict[_roomIdList[index]];
            return new RoomInfo();
        }

        public RoomInfo GetRoomInfoByRoomId(uint id)
        {
            return _lobby.RoomInfoDict[id];
        }
        public void Clear()
        {
            _user = new UserInfo() { UserState = Define.User_State.logout };
            _roomIdList.Clear();
            _lobby = null;
        }
    }
}

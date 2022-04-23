using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

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
            _user = user;
        }
        public void UpdateLobby(LobbyInfo lobbyInfo)
        {
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
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine($"{chat.User.Name} : {chat.Chat}");
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine();
            }
        }
        public bool IsValidRoomNum(int num)
        {
            return _roomIdList?.Count > num;
        }
        public RoomInfo GetRoomInfoByIndex(int index)
        {
            if(IsValidRoomNum(index))
                return _lobby.RoomInfoDict[_roomIdList[index]];
            throw new Exception();
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

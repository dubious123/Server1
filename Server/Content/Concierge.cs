using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ServerCore;
using Server.Data;

namespace Server
{
    public class Concierge
    {
        static Concierge _inst = new Concierge();
        public static Concierge Inst { get { return _inst; } }
        Lobby _lobby;
        ConcurrentDictionary<string, UserInfo> _checkin_dict;
        Concierge()
        {
            _lobby = DataManager.Inst.GetLobby();
            _checkin_dict = new ConcurrentDictionary<string, UserInfo>();
        }
        public LobbyInfo GetLobbyInfo()
        {
            return _lobby.GetInfo();
        }
        public bool CheckUser(UserInfo user)
        {
            if (_checkin_dict.TryGetValue(user.Id, out var original) == false)
                return false;
            return original.Equals(user);       
        }
        public bool TryCheckIn(UserInfo user)
        {
            if (_checkin_dict.ContainsKey(user.Id))
            {
                Console.WriteLine("Concierge : Something is wrong, it's a doppelganger");
                return false;
            }
            user.UserState = Define.User_State.lobby;
            return _checkin_dict.TryAdd(user.Id, user);
        }
        public bool TryCheckOut(UserInfo user)
        {
            if (_checkin_dict.TryRemove(user.Id, out var guest) == false)
            {
                Console.WriteLine("Concierge : Something is wrong, it's a ghost");
                return false;
            }
            return true;
        }
        public void UpdateUserInfo(UserInfo user)
        {
            //todo
            for(int i = 0; i <10; i++)
            {
                if (_checkin_dict.TryGetValue(user.Id, out var info) == false)
                {
                    Console.WriteLine("Invalid user");
                    return;
                }
                if (_checkin_dict.TryUpdate(user.Id, user, info))
                    return;
            }
            Console.WriteLine("User Update Failed");
            throw new Exception();
            
        }
        public bool Check_ReserveRoom(Session session, UserInfo user, uint dest,ref string packetMessage)
        {
            if(user.UserState == Define.User_State.room)
            {
                packetMessage = "You are already in room";
                return false;
            }
            var room = _lobby.GetRoom(dest);
            if(room == null)
            {
                packetMessage = "Invalid roomId";
                return false;
            }
            if(room.Info.CurrentUserNum >= room.Info.MaxUserNum)
            {
                packetMessage = "Room is full";
                return false;
            }
            if (room.TryEnter(session))
            {
                packetMessage = "found doppelganger";
                return false;
            }
            return true;
        }
        public void GuideRoom(UserInfo user, uint dest)
        {
            user.UserState = Define.User_State.room;
            user.RoomId = dest;
        }
        public void GuideLobby(UserInfo user)
        {
            user.UserState = Define.User_State.lobby;
            user.RoomId = 0;
        }
        public Chatting[] GetChatting(uint roomId)
        {
            return _lobby.GetRoom(roomId)?.GetChats();
        }
        public Chatting[] PushChat(uint roomId, Chatting chat)
        {
            var room = _lobby.GetRoom(roomId);
            room.PushChat(chat);
            return room.GetChats();
            
        }


    }
}

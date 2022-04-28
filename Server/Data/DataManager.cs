using ServerCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data
{
    public class DataManager
    {
        static DataManager _inst = new DataManager();
        public static DataManager Inst { get { return _inst; } }
        ConcurrentDictionary<string, UserData> _userDict;
        Lobby _lobby;
        DataManager()
        {
            _userDict = new ConcurrentDictionary<string, UserData>();
            _userDict.TryAdd("jonghun", new UserData("J.Han","jonghun","990827"));
            for(int i = 1; i<501; i++)
            {
                _userDict.TryAdd($"User_{i}", new UserData($"Dummy_{i}",$"User_{i}",$"User_{i * 100}"));
            }
            _lobby = new Lobby();
            _lobby.CreateNewRoom("TestRoom1", 100);
            _lobby.CreateNewRoom("TestRoom2", 100);
            UserInfo user1 = new UserInfo("user1234","user1");
            UserInfo user2 = new UserInfo("user5678", "user2");
            _lobby.GetRoom(1).PushChat(new Chatting(user1,"Hi I'm user1"));
            _lobby.GetRoom(1).PushChat(new Chatting(user2, "Hi I'm user2"));
        }
        public bool GetUserData(string id, out UserData user)
        {
            bool result = _userDict.TryGetValue(id, out UserData inst);
            user = inst;
            return result;
        }
        public Lobby GetLobby()
        {
            return _lobby;
        }
    }
}

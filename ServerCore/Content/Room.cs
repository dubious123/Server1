using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace ServerCore
{
    public class Room
    {
        public RoomInfo Info;
        ConcurrentDictionary<string,Session> _userDict;
        List<Chatting> _chatList;
        public uint ID;
        int _maxChatNum;
        static readonly object _lock = new object();
        public Room()
        {
            _userDict = new ConcurrentDictionary<string, Session>();
            _chatList = new List<Chatting>();
            _maxChatNum = 100;
        }
        public Room(RoomInfo info) : this()
        {
            Info = info;
        }
        public bool TryEnter(string id, Session session)
        {
            if (_userDict.TryAdd(id, session) == false)
                return false;
            Info.CurrentUserNum = (ushort)_userDict.Count;
            return true;
        }
        public bool TryGetOut(string id)
        {
            if (_userDict.TryRemove(id, out var session))
                lock (_lock)
                    Info.CurrentUserNum--;
            return session != null;
        }
        public void PushChat(Chatting chat)
        {
            lock (_lock)
            {
                _chatList.Add(chat);
                if (_chatList.Count >= _maxChatNum)
                    Flush();
            }
        }
        public Chatting[] GetChats()
        {
            Chatting[] block;
            lock (_lock)
            {
                block = _chatList.ToArray();
            }          
            int count = block.Length > 100 ? 100 : block.Length;
            return block[..count];
        }
        public void Flush()
        {
            return;
        }
       

    }
}

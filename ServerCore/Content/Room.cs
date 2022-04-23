using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ServerCore
{
    public class Room
    {
        public RoomInfo Info;
        ConcurrentBag<Session> _userbag;
        List<Chatting> _chatList;
        public uint ID;
        int _maxChatNum;
        static readonly object _lock = new object();
        public Room()
        {
            _userbag = new ConcurrentBag<Session>();
            _chatList = new List<Chatting>();
            _maxChatNum = 100;
        }
        public Room(RoomInfo info) : this()
        {
            Info = info;
        }
        public bool TryEnter(Session user)
        {
            bool result = _userbag.Contains(user);
            if (result == false)
            {
                Console.WriteLine("doppelganger");
                return result;
            }
            _userbag.Add(user);
            return result;
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

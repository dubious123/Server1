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
        Chatting[] _chatArr;
        public uint ID;
        int _maxChatNum;
        int _index;
        static readonly object _lock = new object();
        Action<ArraySegment<Chatting>, uint> _flush;
        public Room(Action<ArraySegment<Chatting>, uint> flush)
        {
            _userDict = new ConcurrentDictionary<string, Session>();
            _maxChatNum = 100;
            _chatArr = new Chatting[_maxChatNum];

            _flush += flush;
        }
        public Room(Action<ArraySegment<Chatting>, uint> flush, RoomInfo info) : this(flush)
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
                _chatArr[_index++] = chat;
                if (_index >= _maxChatNum)
                    Flush();
            }
        }
        public Chatting[] GetChats()
        {
            Chatting[] block;
            lock (_lock)
            {
                block = _chatArr.ToArray();
            }          
            int count = block.Length > 100 ? 100 : block.Length;
            return block[..count];
        }
        void Flush()
        {
            lock (_lock)
            {
                _flush.Invoke(new ArraySegment<Chatting>(_chatArr), ID);
                _chatArr = new Chatting[_maxChatNum];
                _index = 0;
            }

            return;
        }
       

    }
}

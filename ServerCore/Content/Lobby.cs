using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace ServerCore
{
    public class Lobby
    {
        static Lobby _inst = new Lobby();
        public static Lobby Inst { get { return _inst; } }
        ConcurrentDictionary<uint ,Room> _roomDict;
        static readonly object _lock = new object();
        uint _roomCount;
        public Lobby()
        {
            _roomDict = new ConcurrentDictionary<uint, Room>();
        }
        public void CreateNewRoom(RoomInfo roomInfo)
        {
            Room inst = new Room(roomInfo);
            inst.ID = GetNewRoomID();
            if (!_roomDict.TryAdd(inst.ID, inst))
            {
                Console.WriteLine("Invalid room id");
                return;
            }                     
        }
        public void CreateNewRoom(string roomName, ushort maxNum)
        {
            RoomInfo info = new RoomInfo(GetNewRoomID(),roomName, maxNum);
            Room room = new Room(info);
            if (!_roomDict.TryAdd(info.ID,room))
            {
                Console.WriteLine("Invalid room id");
                return;
            }
        }
        public bool DeleteRoom(uint roomId)
        {
            if (!_roomDict.TryRemove(roomId, out Room room))
                return false;           
            return true;
        }
        public Room GetRoom(uint roomId)
        {
            Room room;
            _roomDict.TryGetValue(roomId, out room);
            return room;
        }
        uint GetNewRoomID()
        {
            return Interlocked.Increment(ref _roomCount);
        }
        public LobbyInfo GetInfo()
        {
            LobbyInfo info = new LobbyInfo();
            foreach (var room in _roomDict.Values)
            {
                info.AddRoomInfo(room.Info);
            }
            return info;
        }
    }
}

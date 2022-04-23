using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace ServerCore
{
    public class LobbyInfo
    {
        public Dictionary<uint,RoomInfo> RoomInfoDict = new Dictionary<uint,RoomInfo>();
        public void AddRoomInfo(RoomInfo room)
        {
            RoomInfoDict.Add(room.ID,room);
        }
    }
}

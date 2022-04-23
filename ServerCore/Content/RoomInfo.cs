using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public struct RoomInfo
    {
        public RoomInfo(uint id,string name, ushort max = 100)
        {
            ID = id;
            RoomName = name;
            MaxUserNum = max;
            CurrentUserNum = 0;
        }
        public uint ID;
        public string RoomName;
        public ushort MaxUserNum;
        public ushort CurrentUserNum;
    }
}

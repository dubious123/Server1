using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class UserInfo
    {
        public string Id;
        public string Name;
        public Define.User_State UserState;
        public uint RoomId;
        public UserInfo(string id, string name) : this()
        {
            Id = id;
            Name = name;
        }
        public UserInfo(UserData data) 
        {
            Id = data.Id;
            Name = data.Name;
        }
        public UserInfo(UserInfo user) 
        {
            Id = user.Id;
            Name = user.Name;
            UserState = user.UserState;
            RoomId = user.RoomId;
        }
        public UserInfo()
        {
            UserState = Define.User_State.logout;
            RoomId = 0;
        }
        public override bool Equals(object obj)
        {
            if (obj is UserInfo == false)
                return false;
            var other = (UserInfo)obj;
            if (other.UserState != UserState || other.Name != Name || other.Id != Id)
                return false;
            if (other.UserState == Define.User_State.room)
                if (other.RoomId != RoomId)
                    return false;
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();  
        }
    }
}

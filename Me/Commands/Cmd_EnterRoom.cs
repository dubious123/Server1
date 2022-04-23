using ServerCore;
using ServerCore.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Me
{
    public class Cmd_EnterRoom : Cmd
    {
        int _roomIndex;
        public override bool PutOption(params string[] options)
        {
            if (options.Length > 1)
                return false;
            if (int.TryParse(options[0], out int index) == false)
                return false;
            if (ContentMgr.Inst.IsValidRoomNum(index) == false)
                return false;
            _roomIndex = index;
            return true;
        }
        public override void Start()
        {
            if (CheckUserCondition() == false)
            {
                Console.WriteLine("You are not in lobby");
                CmdMgr.Inst.Dequeue();
                return;
            }
            Console.WriteLine();
            Console.Write($"Entering Room {_roomIndex} ");
            uint roomId = ContentMgr.Inst.GetRoomInfoByIndex(_roomIndex).ID;
            C_EnterRoom c_packet = new C_EnterRoom(ContentMgr.Inst.User, roomId);
            SessionMgr.Inst.Find(1).RegisterSend(c_packet);
            CmdMgr.Inst.UpdateState(Define.Cmd_State.wait);
        }
        public override void Done()
        {
            Console.WriteLine();
            Console.WriteLine($"Entered Room {_roomIndex} : {ContentMgr.Inst.GetRoomInfoByIndex(_roomIndex).RoomName}");
            CmdMgr.Inst.Dequeue();
            CmdMgr.Inst.Enqueue(new Cmd_UpdateRoom());
        }

        public override void Error()
        {
            throw new NotImplementedException();
        }
        public override bool CheckUserCondition()
        {
            return ContentMgr.Inst.User.UserState == Define.User_State.lobby;
        }


  
    }
}

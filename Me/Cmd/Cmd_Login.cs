using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;

namespace Me
{
    public class Cmd_Login : Cmd
    {
        public override void Start()
        {
            Console.Write("ID : ");
            var id = Console.ReadLine();
            Console.Write("Password : ");
            var pw = Console.ReadLine();
            Console.WriteLine("");
            if (CheckID(id) && CheckPW(pw) == false)
            {
                _errorMessage = "Invalid ID or PW";
                UpdateState(Define.Cmd_State.error);
                return;
            }
            C_LoginPacket loginPacket = new C_LoginPacket(id, pw);
            var packet = PacketMgr.Inst.PacketToByte(loginPacket);
            SessionMgr.Inst.Find(1).RegisterSend(packet);
            Console.Write("Waiting");
            UpdateState(Define.Cmd_State.wait);
        }
        public override void Done()
        {
            Console.WriteLine("\n Login Completed ");
            CmdMgr.Inst.Dequeue();
        }

        public override void Error()
        {
            Console.WriteLine("\n Login Failed : "+ _errorMessage);
            CmdMgr.Inst.Dequeue();
        }


        static bool CheckID(string id)
        {
            return (id.Length < 15 && id.Length > 0);
        }
        static bool CheckPW(string pw)
        {
            return (pw.Length < 15 && pw.Length > 0);
        }


    }
}

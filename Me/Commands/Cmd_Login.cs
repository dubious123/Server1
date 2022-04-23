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
        public override bool PutOption(params string[] options)
        {
            return false;
        }
        public override void Start()
        {
            if (CheckUserCondition() == false)
            {
                Console.WriteLine("already logged in");
                CmdMgr.Inst.Dequeue();
                return;
            }
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
            C_Login loginPacket = new C_Login(id, pw);
            var packet = PacketMgr.Inst.PacketToByte(loginPacket);
            SessionMgr.Inst.Find(1).RegisterSend(packet);
            Console.Write("Waiting");
            UpdateState(Define.Cmd_State.wait);
        }
        public override void Done()
        {
            Console.WriteLine("\n Login Completed \n");
            Console.WriteLine($"Hello {ContentMgr.Inst.User.Name}");
            CmdMgr.Inst.Dequeue();
            CmdMgr.Inst.Enqueue(new Cmd_EnterLobby());
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

        public override bool CheckUserCondition()
        {
            return ContentMgr.Inst.User.UserState == Define.User_State.logout;
        }
    }
}

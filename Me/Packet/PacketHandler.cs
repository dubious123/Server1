using PacketTools;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Me
{
    public class PacketHandler
    {
        static PacketHandler _instance = new PacketHandler();
        public static PacketHandler Inst { get { return _instance; } }
        PacketHandler()
        {
            _handlerDict = new Dictionary<ushort, Action<IPacket, Session>>();
            _handlerDict.Add((ushort)Define.PacketId.s_login, HandleLogin);
        }
        Dictionary<ushort, Action<IPacket, Session>> _handlerDict;
        public void HandlePacket<T, P>(T packet, P session) where T : IPacket where P : Session
        {
            Action<IPacket, Session> action;
            _handlerDict.TryGetValue(packet.PacketId, out action);
            action?.Invoke(packet, session);
        }
        void HandleLogin(IPacket packet, Session session)
        {
            var s_packet = packet as S_LoginPacket;
            if (s_packet.Accepted)
                CmdMgr.Inst.UpdateState(Define.Cmd_State.done);
            else
                CmdMgr.Inst.UpdateState(Define.Cmd_State.error);

        }
        void HandleTest1(IPacket packet, Session session)
        {
            var testPacket = packet as TestPacket1;
            //Console.WriteLine("Handling TestPacket1");

            //Console.WriteLine($"id : {testPacket.Name} chat : {testPacket.Chat}");

            TestPacket1 callback = new TestPacket1 { Name = "Client", Chat = "Hello From Client", PacketId = 1 };

            session.RegisterSend(PacketMgr.Inst.PacketToByte(callback));
        }

    }
}

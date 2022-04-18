using PacketTools;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class PacketHandler
    {
        static PacketHandler _instance = new PacketHandler();
        public static PacketHandler Inst { get { return _instance; } }
        ManualResetEvent _resetEvent = new ManualResetEvent(false);
        PacketHandler()
        {
            _handlerDict = new Dictionary<ushort, Action<IPacket, Session>>();
            _handlerDict.Add(1, HandleTest1);
        }
        Dictionary<ushort, Action<IPacket, Session>> _handlerDict;
        public void HandlePacket<T, P>(T packet, P session) where T : IPacket where P : Session
        {
            Action<IPacket, Session> action;
            _handlerDict.TryGetValue(packet.PacketId, out action);
            action?.Invoke(packet, session);
        }
        void HandleTest1(IPacket packet, Session session)
        {
            var testPacket = packet as TestPacket1;
            //Console.WriteLine("Handling TestPacket1");

            //Console.WriteLine($"id : {testPacket.Name} chat : {testPacket.Chat}");

            TestPacket1 callback = new TestPacket1 { Name = "Server", Chat = "Hello From Server", PacketId = 1 };
            byte[] json = null;
            JobMgr.Inst.Push("Json", () => 
            {
                PacketMgr.Inst.PacketToByte(callback,out json);
                _resetEvent.Set();
            }
            );
            //Do Something

            _resetEvent.WaitOne();
            JobMgr.Inst.Push("Send", () => session.RegisterSend(json));
        }

    }
}

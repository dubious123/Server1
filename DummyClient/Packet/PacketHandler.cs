using PacketTools;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace DummyClient
{
    public class PacketHandler
    {
        static PacketHandler _instance = new PacketHandler();
        public static PacketHandler Inst { get { return _instance; } }
        PacketHandler()
        {
            _handlerDict = new Dictionary<ushort, Action<IPacket, Session>>();
        }
        Dictionary<ushort, Action<IPacket, Session>> _handlerDict;
        public void HandlePacket<T, P>(T packet, P session) where T : IPacket where P : Session
        {
            Action<IPacket, Session> action;
            _handlerDict.TryGetValue(packet.PacketId, out action);
            action?.Invoke(packet, session);
        }


    }
}

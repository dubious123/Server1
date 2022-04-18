using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ServerCore;
using PacketTools;

namespace DummyClient
{
    public class ServerSession : Session
    {
        public override void OnDisconnect()
        {
            throw new NotImplementedException();
        }

        public override void OnReceive(SocketAsyncEventArgs args)
        {
            var packets = PacketMgr.Inst.ByteToPacket(_recvBuff);
            Console.WriteLine($"모아받은 패킷 수 : {packets.Count}");
            foreach (var packet in packets)
            {
                PacketHandler.Inst.HandlePacket(packet, this);
            }
            
 
        }

        public override void OnSend(SocketAsyncEventArgs args)
        {
            Console.WriteLine($"To {args.RemoteEndPoint} Sent {args.BytesTransferred}");
        }
    }
}

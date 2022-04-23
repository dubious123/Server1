using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Me
{
    public class ServerSession : Session
    {
        public override void OnConnected()
        {
            Console.WriteLine($"From server endpoint {_socket.RemoteEndPoint} Connected");
            RegisterReceive();
        }

        public override void OnDisconnect()
        {
            Console.WriteLine($"From server endpoint {_socket.RemoteEndPoint} DisConnected");
        }

        public override void OnReceive(SocketAsyncEventArgs args)
        {
            var packets = PacketMgr.Inst.ByteToPacket(_recvBuff);
            //Console.WriteLine($"모아받은 패킷 수 : {packets.Count}");
            JobMgr.Inst.Push("PacketHandle", () =>
            {
                foreach (var packet in packets)
                {
                    PacketHandler.Inst.HandlePacket(packet, this);
                }
            });
        }
        public override void OnSend(SocketAsyncEventArgs args)
        {
            //Console.WriteLine($"To {_socket.RemoteEndPoint} Sent {args.BytesTransferred}");
        }
        public override void OnReceiveFailed(Exception ex)
        {
            throw new NotImplementedException();
        }

        public override void OnSendFailed(Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}

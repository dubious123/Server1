using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using DummyClient.Packet;
using ServerCore;

namespace DummyClient
{
    public class D_ServerSession : Session
    {
        public override void OnConnected()
        {
            RegisterReceive();
        }

        public override void OnDisconnect()
        {
        }

        public override void OnReceive(SocketAsyncEventArgs args)
        {
            var packets = PacketMgr.Inst.ByteToPacket(_recvBuff);
            //Console.WriteLine($"모아받은 패킷 수 : {packets.Count}");
            JobMgr.Inst.Push("PacketHandle", () =>
            {
                foreach (var packet in packets)
                {
                    D_PacketHandler.Inst.HandlePacket(packet, this);
                }
            });
        }
        public override void OnSend(SocketAsyncEventArgs args)
        {
            //Console.WriteLine($"To {_socket.RemoteEndPoint} Sent {args.BytesTransferred}");
        }
        public override void OnReceiveFailed(Exception ex)
        {
            CloseSession();
        }

        public override void OnSendFailed(Exception ex)
        {
            if (_socket.Connected == false)
            {
            }
            CloseSession();
        }
    }
}

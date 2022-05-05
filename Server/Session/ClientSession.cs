using PacketTools;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ClientSession : Session
    {
        public string UserId;
        public override void OnConnected()
        {
            Console.WriteLine($"From client endpoint {_socket?.RemoteEndPoint} Connected");
            RegisterReceive();
        }

        public override void OnDisconnect()
        {
            _sendRegistered = false;
            if(_socket.Connected)
                Console.WriteLine($"From client endpoint {_socket?.RemoteEndPoint} DisConnected");
            Gatekeeper.Inst.TryLogout(UserId);
            SessionMgr.Inst.Remove(SessionID);
        }


        public override void OnReceive(SocketAsyncEventArgs args)
        {
            var packets = PacketMgr.Inst.ByteToPacket(_recvBuff);
            foreach (var packet in packets)
            {
                PacketHandler.Inst.HandlePacket(packet, this);
            }
            Console.WriteLine($"모아받은 패킷 수 : {packets.Count}");
            //JobMgr.Inst.Push("PacketHandle", () =>
            //{
            //    foreach (var packet in packets)
            //    {
            //        PacketHandler.Inst.HandlePacket(packet, this);
            //    }
            //});

        }



        public override void OnSend(SocketAsyncEventArgs args)
        {
            Console.WriteLine($"To {args.RemoteEndPoint} Sent {args.BytesTransferred}");
        }
        public override void OnReceiveFailed(Exception ex)
        {
            Console.WriteLine($"Receiving packet failed");
            CloseSession();
        }
        public override void OnSendFailed(Exception ex)
        {
            Console.WriteLine($"Sending packet failed");
            CloseSession();
        }
    }
}

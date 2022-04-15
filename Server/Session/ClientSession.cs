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
        public override void OnDisconnect()
        {
            throw new NotImplementedException();
        }


        public override void OnReceive(SocketAsyncEventArgs args)
        {
            var packets = PacketMgr.Inst.ByteToPacket(_recvBuff);
            Console.WriteLine($"From {_socket.RemoteEndPoint} Received {args.BytesTransferred}");

            Console.WriteLine($"Number of receive packets : {packets.Count}");
            foreach (var packet in packets)
            {
                
                
                if(packet is TestPacket1)
                {
                    var t = packet as TestPacket1;
                    Console.WriteLine($"Name : {t.Name}");
                    Console.WriteLine($"Caht : {t.Chat}");
                }


            }

        }


        public override void OnSend(SocketAsyncEventArgs args)
        {
            Console.WriteLine($"To {args.RemoteEndPoint} Sent {args.BytesTransferred}");
        }
    }
}

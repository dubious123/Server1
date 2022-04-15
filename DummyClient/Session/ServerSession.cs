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
            Console.WriteLine($"From {_socket.RemoteEndPoint} Received {args.BytesTransferred}");
            //var receiePacket = PacketSerializer.DeSerialize_Json<TestPacket1>(_recvBuff.Array);
 
        }

        public override void OnSend(SocketAsyncEventArgs args)
        {
            Console.WriteLine($"To {args.RemoteEndPoint} Sent {args.BytesTransferred}");
        }
    }
}

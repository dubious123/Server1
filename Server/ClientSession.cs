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
            Console.WriteLine($"From {args.RemoteEndPoint} Received {args.BytesTransferred}");
        }


        public override void OnSend(SocketAsyncEventArgs args)
        {
            Console.WriteLine($"To {args.RemoteEndPoint} Sent {args.BytesTransferred}");
        }
    }
}

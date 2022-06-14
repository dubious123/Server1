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
            RegisterReceive();
        }

        public override void OnDisconnect()
        {
        }

        public override void OnReceive(SocketAsyncEventArgs args)
        {
            foreach (var packet in PacketMgr.Inst.ByteToPacket(_recvBuff))
            {
                PacketHandler.Inst.HandlePacket(packet, this);
            }
        }
        public override void OnSend(SocketAsyncEventArgs args)
        {
            //Console.WriteLine($"To {_socket.RemoteEndPoint} Sent {args.BytesTransferred}");
        }
        public override void OnReceiveFailed(Exception ex)
        {
            CmdMgr.Inst.Clear();
            CloseSession();
        }

        public override void OnSendFailed(Exception ex)
        {
            if(_socket.Connected == false)
            {
            }
            CmdMgr.Inst.Clear();
            CloseSession();
        }
    }
}

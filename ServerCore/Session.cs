using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections.Concurrent;
using PacketTools;
using System.Threading;

namespace ServerCore
{
    public abstract class Session
    {
        public abstract void OnSend(SocketAsyncEventArgs args);
        public abstract void OnReceive(SocketAsyncEventArgs args);
        public abstract void OnDisconnect();

        protected Socket _socket;
        protected SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        protected SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        protected RecvBuffer _recvBuff = new RecvBuffer(40490);
        protected SendBuffer _sendBuff = new SendBuffer();
        protected List<ArraySegment<byte>> _sendList = new List<ArraySegment<byte>>();
        public void Clear()
        {
            _sendList.Clear();
        }
        public void Init(Socket socket)
        {
            _socket = socket;
            _recvArgs.Completed += OnReceiveCompleted;
            _sendArgs.Completed += OnSendCompleted;
        }
        //시작점
        public void OnConnected()
        {
            //Test
            TestPacket1 packet = new TestPacket1(1, "Jonghun", "Hello");
            var segment = PacketMgr.Inst.PacketToByte(packet);
            for(int i = 0; i < 100; i++)
            {
                RegisterSend(segment);
            }
            
            RegisterReceive();
        }
        #region Send
        //todo -> lock free
        static readonly object _lock = new object();
        public void RegisterSend(ArraySegment<byte> packet)
        {
            lock (_lock)
            {
                _sendBuff.Write(packet);
            }
            
            JobMgr.Inst.Push("Send", Send);
        }
        bool pending = false;
        void Send()
        {
            if (pending)
                return;
            var list = _sendBuff.ReadAll();
            if (list.Count == 0)
                return;

            _sendArgs.BufferList = new List<ArraySegment<byte>>();
            _sendArgs.BufferList = list;
            Console.WriteLine($"Sending {_sendArgs.BufferList.Count} Packets");
            pending = _socket.SendAsync(_sendArgs);
            if (!pending)
                OnSendCompleted(null, _sendArgs);


        }
        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                OnSend(args);
            }
            else
            {
                _socket.Close();
            }
        }

        #endregion

        #region Receive
        public void RegisterReceive()
        {
            _recvBuff.Clear();
            _recvArgs.SetBuffer(_recvBuff.WriteSegment);
            bool pending =_socket.ReceiveAsync(_recvArgs);
            if (!pending)
                OnReceiveCompleted(null, _recvArgs);
        }
        void OnReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                if (!_recvBuff.OnWrite(args.BytesTransferred))
                {
                    Console.WriteLine("RecvBuff don't have enough space");
                    return;
                }
                OnReceive(args);
                RegisterReceive();
            }
            else
                CloseSession();

        }
        #endregion

        public void CloseSession()
        {

        }
    }
}

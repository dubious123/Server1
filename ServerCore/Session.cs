using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace ServerCore
{
    public abstract class Session
    {
        public abstract void OnSend(SocketAsyncEventArgs args);
        public abstract void OnReceive(SocketAsyncEventArgs args);
        public abstract void OnDisconnect();

        Socket _socket;
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        ArraySegment<byte> _recvBuff = new ArraySegment<byte>(new byte[4096]);
        Queue<ArraySegment<byte>> _sendQueue = new Queue<ArraySegment<byte>>();
        List<ArraySegment<byte>> _sendList = new List<ArraySegment<byte>>();

        object _lock = new object();
        public void Clear()
        {
            _sendQueue.Clear();
            _sendList.Clear();
        }
        public void Init(Socket socket)
        {
            _socket = socket;
            _recvArgs.Completed += OnReceiveCompleted;
            _sendArgs.Completed += OnSendCompleted;
        }
        
        #region Send
        //todo -> lock free
        public void RegisterSend(ArraySegment<byte> packet)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(packet);
                Send();
            }
        }
        void Send()
        {
            while (_sendQueue.Count > 0)
            {
                _sendList.Add(_sendQueue.Dequeue());
            }
            _sendArgs.BufferList = _sendList;
            bool pending = _socket.SendAsync(_sendArgs);
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
            _recvArgs.SetBuffer(_recvBuff);
            bool pending =_socket.ReceiveAsync(_recvArgs);
            if (!pending)
                OnReceiveCompleted(null, _recvArgs);
        }
        void OnReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
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

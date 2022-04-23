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
        public abstract void OnConnected();
        public abstract void OnSendFailed(Exception ex);
        public abstract void OnReceiveFailed(Exception ex);
        public uint SessionID;
        public bool SendRegistered { get { return _sendRegistered; } }

        protected bool _sendRegistered = false;
        protected Socket _socket;
        protected SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        protected SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        protected RecvBuffer _recvBuff = new RecvBuffer(40490);
        protected SendBuffer _sendBuff = new SendBuffer();
        protected List<ArraySegment<byte>> _sendList = new List<ArraySegment<byte>>();
        public void Clear()
        {
            lock (_send)
            {
                _sendList.Clear();
                _recvBuff.Clear();
                _sendBuff.Clear();
            }          
        }
        public void Init(Socket socket)
        {
            _socket = socket;
            _recvArgs.Completed += OnReceiveCompleted;
            _sendArgs.Completed += OnSendCompleted;
        }
        //시작점
        #region Send
        //todo -> lock free
        static readonly object _send = new object();
        public void RegisterSend(IPacket packet)
        {
            RegisterSend(PacketMgr.Inst.PacketToByte(packet));
        }
        public void RegisterSend(ArraySegment<byte> packet)
        {
            _sendBuff.Write(packet);
            if (!_sendRegistered)
                _sendRegistered = true;
        }
        bool _sendPending = false;

        /// <summary>
        /// Do not use directly
        /// </summary>
        public void Send()
        {

            if (_sendPending)
                return;
            var list = _sendBuff.ReadAll();
            if (list.Count == 0)
                return;
            _sendRegistered = false;
            _sendArgs.BufferList = list;
            //Console.WriteLine($"From Session {SessionID} Sending {_sendArgs.BufferList.Count} Packets");
            try
            {
                _sendPending = _socket.SendAsync(_sendArgs);
                if (!_sendPending)
                    OnSendCompleted(null, _sendArgs);
            }
            catch (Exception ex)
            {
                OnSendFailed(ex);
            }
        }
        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                OnSend(args);
            }
            else
            {
                OnSendFailed(null);
            }
        }

        #endregion

        #region Receive
        static readonly object _receive = new object();
        public void RegisterReceive()
        {
            lock (_receive)
            {
                _recvBuff.Clear();             
            }
            _recvArgs.SetBuffer(_recvBuff.WriteSegment);
            try
            {
                bool pending = _socket.ReceiveAsync(_recvArgs);
                if (!pending)
                    OnReceiveCompleted(null, _recvArgs);
            }
            catch (Exception ex)
            {
                OnReceiveFailed(ex);
            }
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
                OnReceiveFailed(null);

        }
        #endregion

        public void CloseSession()
        {
            OnDisconnect();
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            _socket.Close();
            
            Clear();
        }
    }
}

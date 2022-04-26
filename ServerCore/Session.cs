using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Collections.Concurrent;
using PacketTools;
using System.Threading;
using System.Diagnostics;
using ServerCore.Log;

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

        TraceSource _ts;
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

            _ts = LogMgr.GetTraceSource("Session");
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
            _ts.TraceInfo($"[Session {SessionID}] : registered send {packet.Count} bytes");
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
            _ts.TraceInfo($"[Session {SessionID}] : try sending");
            if (_sendPending)
            {
                _ts.TraceEvent(TraceEventType.Warning, 2, $"[Session {SessionID}] : sending canceled : session is pending");
                return;
            }
                
            var list = _sendBuff.ReadAll();
            if (list.Count == 0)
            {            
                _ts.TraceEvent(TraceEventType.Warning, 3, $"[Session {SessionID}] : sending canceled : nothing to send");
                return;
            }

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
                _ts.TraceEvent(TraceEventType.Warning, 4 ,$"[Session {SessionID}] : sending failed before sending\n" +
                    $"error : {ex}");
                OnSendFailed(ex);
            }
        }
        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            if(args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                _ts.TraceInfo($"[Session {SessionID}] : sending {args.BytesTransferred} bytes completed");
                OnSend(args);
            }
            else
            {
                _ts.TraceEvent(TraceEventType.Warning, 5, $"[Session {SessionID}] : sending failed after sending\n" +
                    $"socket errer :{args.SocketError}, transferred bytes : {args.BytesTransferred}/ ");
                OnSendFailed(null);
            }
        }

        #endregion

        #region Receive
        static readonly object _receive = new object();
        public void RegisterReceive()
        {
            _ts.TraceInfo($"[Session {SessionID}] : Register receive");
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
                _ts.TraceEvent(TraceEventType.Warning, 6, $"[Session {SessionID}] : receiving failed before receive\n" +
                        $"error : {ex} ");
                OnReceiveFailed(ex);
            }
        }
        void OnReceiveCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success && args.BytesTransferred > 0)
            {
                _ts.TraceInfo($"[Session {SessionID}] : receive {args.BytesTransferred} bytes completed");
                if (!_recvBuff.OnWrite(args.BytesTransferred))
                {
                    _ts.TraceEvent(TraceEventType.Error, 7, $"[Session {SessionID}] : there is not enough space in recvBuff and received bytes ignored");
                    RegisterReceive();
                    return;
                }
                OnReceive(args);
                RegisterReceive();
            }
            else
            {
                _ts.TraceEvent(TraceEventType.Warning, 8, $"[Session {SessionID}] : receive failed after receive\n" +
    $"socket errer :{args.SocketError}, transferred bytes : {args.BytesTransferred}/ ");
                OnReceiveFailed(null);
            }
                

        }
        #endregion

        public void CloseSession()
        {
            _ts.TraceInfo($"[Session {SessionID}] : Closing session");
            OnDisconnect();
            try
            {
                _ts.TraceInfo($"[Session {SessionID}] : Shutting down");
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch(Exception ex)
            {
                _ts.TraceEvent(TraceEventType.Error, 9, $"[Session {SessionID}] : shut down failed " +
                    $"error : {ex}");
            }
            _socket.Close();
            
            Clear();
        }
        public bool GetSocketState()
        {
            return _socket.Connected;
        }
    }
}

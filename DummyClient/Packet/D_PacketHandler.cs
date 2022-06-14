using PacketTools;
using ServerCore;
using ServerCore.Log;
using ServerCore.Packets;
using ServerCore.Packets.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient.Packet
{
    public class D_PacketHandler
    {
        static D_PacketHandler _instance = new D_PacketHandler();
        public static D_PacketHandler Inst { get { return _instance; } }
        Dictionary<ushort, Action<BasePacket, Session>> _handlerDict;
        TraceSource _ts;
        D_PacketHandler()
        {
            _ts = LogMgr.AddNewSource("Packet", SourceLevels.Information);
            LogMgr.AddNewTextWriterListener("Packet", "t_packet", "Packet.txt", SourceLevels.Information, TraceOptions.DateTime);

            _handlerDict = new Dictionary<ushort, Action<BasePacket, Session>>();
            _handlerDict.Add((ushort)Define.P_Id.s_login, HandleLogin);
            _handlerDict.Add((ushort)Define.P_Id.s_logout, HandleLogout);
            _handlerDict.Add((ushort)Define.P_Id.s_enterLobby, HandleEnterLobby);
            _handlerDict.Add((ushort)Define.P_Id.s_updateLobby, HandleUpdateLobby);
            _handlerDict.Add((ushort)Define.P_Id.s_enterRoom, HandleEnterRoom);
            _handlerDict.Add((ushort)Define.P_Id.s_exitRoom, HandleExitRoom);
            _handlerDict.Add((ushort)Define.P_Id.s_updateRoom, HandleUpdateRoom);
        }

        public void HandlePacket<T, P>(T packet, P session) where T : BasePacket where P : Session
        {
            if (_handlerDict.TryGetValue(packet.PacketId, out Action<BasePacket, Session> action) == false)
            {
                _ts.TraceEvent(TraceEventType.Warning, 0, $"[PacketHandler] Invalid packetId from session {session.SessionID}");
                return;
            }
            action.Invoke(packet, session);
        }
        void HandleLogin(BasePacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling Login packet from session {session.SessionID}");
            if (CheckPacketCasting<S_Login>(packet, session) == false)
                return;
            var dummy = DummyMgr.Inst.GetDummies(session.SessionID);
            var s_packet = packet as S_Login;
            if (!s_packet.Accepted)
            {
                dummy.UpdateState(Define.Cmd_State.error);
                return;
            }
            dummy.UpdateUserInfo(s_packet.User);
            dummy.UpdateState(Define.Cmd_State.start);
        }
        void HandleLogout(BasePacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling Logout packet from session {session.SessionID}");
            if (CheckPacketCasting<S_Logout>(packet, session) == false)
                return;
            var dummy = DummyMgr.Inst.GetDummies(session.SessionID);
            var s_pacekt = packet as S_Logout;
            if (s_pacekt.Answer == false)
            {
                dummy.UpdateState(Define.Cmd_State.error, s_pacekt.Message);
                return;
            }
            dummy.ResetDummy();
        }
        void HandleEnterLobby(BasePacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling EnterLobby packet from session {session.SessionID}");
            if (CheckPacketCasting<S_EnterLobby>(packet, session) == false)
                return;
            var s_packet = packet as S_EnterLobby;
            var dummy = DummyMgr.Inst.GetDummies(session.SessionID);
            dummy.UpdateUserInfo(s_packet.User);
            if (!s_packet.Accept)
            {
                dummy.UpdateState(Define.Cmd_State.error, s_packet.Message);
                //logout
                return;
            }
            dummy.UpdateState(Define.Cmd_State.start);
        }
        void HandleUpdateLobby(BasePacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling UpdateLobby packet from session {session.SessionID}");
            if (CheckPacketCasting<S_UpdateLobby>(packet, session) == false)
                return;
            var s_packet = packet as S_UpdateLobby;
            var dummy = DummyMgr.Inst.GetDummies(session.SessionID);
            dummy.UpdateLobby(s_packet.Info);

            dummy.UpdateState(Define.Cmd_State.start);
        }
        void HandleEnterRoom(BasePacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling EnterRoom packet from session {session.SessionID}");
            if (CheckPacketCasting<S_EnterRoom>(packet, session) == false)
                return;
            var s_packet = packet as S_EnterRoom;
            var dummy = DummyMgr.Inst.GetDummies(session.SessionID);
            //if (s_packet.Answer == false)
            //{
            //    dummy.UpdateState(Define.Cmd_State.error, s_packet.Message);
            //    return;
            //}
            dummy.UpdateUserInfo(s_packet.User);
            dummy.UpdateState(Define.Cmd_State.start);

        }
        void HandleExitRoom(BasePacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling ExitRoom packet from session {session.SessionID}");
            if (CheckPacketCasting<S_ExitRoom>(packet, session) == false)
                return;
            var s_packet = packet as S_ExitRoom;
            var dummy = DummyMgr.Inst.GetDummies(session.SessionID);
            if (s_packet.Answer == false)
            {
                dummy.UpdateState(Define.Cmd_State.error, s_packet.Message);
                return;
            }
            dummy.UpdateUserInfo(s_packet.User);
            dummy.UpdateState(Define.Cmd_State.start);
        }
        void HandleUpdateRoom(BasePacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling UpdateRoom packet from session {session.SessionID}");
            if (CheckPacketCasting<S_UpdateRoom>(packet, session) == false)
                return;
            var dummy = DummyMgr.Inst.GetDummies(session.SessionID);
            //var s_packet = packet as S_UpdateRoom;
            dummy.UpdateState(Define.Cmd_State.start);
        }
        bool CheckPacketCasting<T>(BasePacket packet, Session session) where T : BasePacket
        {
            if (packet is T)
                return true;
            _ts.TraceEvent(TraceEventType.Critical, -1, $"[PacketHandler] got invalid packet from session {session.SessionID}");
            return false;
        }
    }
}

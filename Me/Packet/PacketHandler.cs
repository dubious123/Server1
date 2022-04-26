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

namespace Me
{
    public class PacketHandler
    {
        static PacketHandler _instance = new PacketHandler();
        public static PacketHandler Inst { get { return _instance; } }
        Dictionary<ushort, Action<IPacket, Session>> _handlerDict;
        TraceSource _ts;
        PacketHandler()
        {
            _ts = LogMgr.AddNewSource("Packet", SourceLevels.Information);
            LogMgr.AddNewTextWriterListener("Packet", "t_packet", "Packet.txt", SourceLevels.Information, TraceOptions.DateTime);

            _handlerDict = new Dictionary<ushort, Action<IPacket, Session>>();
            _handlerDict.Add((ushort)Define.P_Id.s_login, HandleLogin);
            _handlerDict.Add((ushort)Define.P_Id.s_logout, HandleLogout);
            _handlerDict.Add((ushort)Define.P_Id.s_enterLobby, HandleEnterLobby);
            _handlerDict.Add((ushort)Define.P_Id.s_updateLobby, HandleUpdateLobby);
            _handlerDict.Add((ushort)Define.P_Id.s_enterRoom, HandleEnterRoom);
            _handlerDict.Add((ushort)Define.P_Id.s_exitRoom, HandleExitRoom);
            _handlerDict.Add((ushort)Define.P_Id.s_updateRoom, HandleUpdateRoom);
        }

        public void HandlePacket<T, P>(T packet, P session) where T : IPacket where P : Session
        {
            if (_handlerDict.TryGetValue(packet.PacketId, out Action<IPacket, Session> action) == false)
            {
                _ts.TraceEvent(TraceEventType.Warning, 0, $"[PacketHandler] Invalid packetId from session {session.SessionID}");
                return;
            }
            action.Invoke(packet, session);
        }
        void HandleLogin(IPacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling Login packet from session {session.SessionID}");
            if (CheckPacketCasting<S_Login>(packet, session) == false)
                return;
            var s_packet = packet as S_Login;
            if (!s_packet.Accepted)
            {
                CmdMgr.Inst.UpdateState(Define.Cmd_State.error);
                return;
            }
            ContentMgr.Inst.UpdateUserInfo(s_packet.User);
            CmdMgr.Inst.UpdateState(Define.Cmd_State.done);
        }
        void HandleLogout(IPacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling Logout packet from session {session.SessionID}");
            if (CheckPacketCasting<S_Logout>(packet, session) == false)
                return;
            var s_pacekt = packet as S_Logout;
            if(s_pacekt.Answer == false)
            {
                CmdMgr.Inst.UpdateState(Define.Cmd_State.error, s_pacekt.Message);
                return;
            }
            CmdMgr.Inst.UpdateState(Define.Cmd_State.done);
        }
        void HandleEnterLobby(IPacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling EnterLobby packet from session {session.SessionID}");
            if (CheckPacketCasting<S_EnterLobby>(packet, session) == false)
                return;
            var s_packet = packet as S_EnterLobby;
            ContentMgr.Inst.UpdateUserInfo(s_packet.User);
            if (!s_packet.Accept)
            {
                CmdMgr.Inst.UpdateState(Define.Cmd_State.error);
                //logout
                return;
            }
            CmdMgr.Inst.UpdateState(Define.Cmd_State.done);
        }
        void HandleUpdateLobby(IPacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling UpdateLobby packet from session {session.SessionID}");
            if (CheckPacketCasting<S_UpdateLobby>(packet, session) == false)
                return;
            var s_packet = packet as S_UpdateLobby;

            ContentMgr.Inst.UpdateLobby(s_packet.Info);
            
            CmdMgr.Inst.UpdateState(Define.Cmd_State.done);
        }
        void HandleEnterRoom(IPacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling EnterRoom packet from session {session.SessionID}");
            if (CheckPacketCasting<S_EnterRoom>(packet, session) == false)
                return;
            var s_packet = packet as S_EnterRoom;
            if (s_packet.Answer == false)
            {
                CmdMgr.Inst.UpdateState(Define.Cmd_State.error, s_packet.Message);
                return;
            }
            ContentMgr.Inst.UpdateUserInfo(s_packet.User);
            CmdMgr.Inst.UpdateState(Define.Cmd_State.done);

        }
        void HandleExitRoom(IPacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling ExitRoom packet from session {session.SessionID}");
            if (CheckPacketCasting<S_ExitRoom>(packet, session) == false)
                return;
            var s_packet = packet as S_ExitRoom;
            if(s_packet.Answer == false)
            {
                CmdMgr.Inst.UpdateState(Define.Cmd_State.error, s_packet.Message);
                return;
            }
            ContentMgr.Inst.UpdateUserInfo(s_packet.User);
            CmdMgr.Inst.UpdateState(Define.Cmd_State.done);
        }
        void HandleUpdateRoom(IPacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling UpdateRoom packet from session {session.SessionID}");
            if (CheckPacketCasting<S_UpdateRoom>(packet, session) == false)
                return;
            var s_packet = packet as S_UpdateRoom;
            ContentMgr.Inst.ShowChats(s_packet.Chats);
            CmdMgr.Inst.UpdateState(Define.Cmd_State.done);
        }
        bool CheckPacketCasting<T>(IPacket packet, Session session) where T : IPacket
        {
            if (packet is T)
                return true;
            _ts.TraceEvent(TraceEventType.Critical, -1, $"[PacketHandler] got invalid packet from session {session.SessionID}");
            return false;
        }
    }
}

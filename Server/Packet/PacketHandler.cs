using PacketTools;
using ServerCore;
using ServerCore.Packets;
using ServerCore.Packets.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore.Packets.Server;
using System.Diagnostics;
using ServerCore.Log;

namespace Server
{
    public class PacketHandler
    {
        static PacketHandler _instance = new PacketHandler();
        public static PacketHandler Inst { get { return _instance; } }
        TraceSource _ts;
        PacketHandler()
        {
            _ts = LogMgr.AddNewSource("Packet", SourceLevels.Information);
            LogMgr.AddNewTextWriterListener("Packet", "t_packet", "Packet.txt", SourceLevels.Information, TraceOptions.DateTime);

            _handlerDict = new Dictionary<ushort, Action<IPacket, Session>>();
            _handlerDict.Add((ushort)Define.P_Id.c_login, HandleLogin);
            _handlerDict.Add((ushort)Define.P_Id.c_logout, HandleLogout);
            _handlerDict.Add((ushort)Define.P_Id.c_enterLobby, HandleEnterLobby);
            _handlerDict.Add((ushort)Define.P_Id.c_updateLobby, HandleUpdateLobby);
            _handlerDict.Add((ushort)Define.P_Id.c_enterRoom, HandleEnterRoom);
            _handlerDict.Add((ushort)Define.P_Id.c_exitRoom, HandleExitRoom);
            _handlerDict.Add((ushort)Define.P_Id.c_updateRoom, HandleUpdateRoom);
            _handlerDict.Add((ushort)Define.P_Id.c_chat, HandleChat);

        }
        Dictionary<ushort, Action<IPacket, Session>> _handlerDict;
        public void HandlePacket<T, P>(T packet, P session) where T : IPacket where P : Session
        {       
            if(_handlerDict.TryGetValue(packet.PacketId, out Action<IPacket, Session> action) == false)
            {
                _ts.TraceEvent(TraceEventType.Warning, 0, $"[PacketHandler] Invalid packetId from session {session.SessionID}");
                return;
            }
            action.Invoke(packet, session);
        }
        void HandleLogin(IPacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling Login packet from session {session.SessionID}");
            if (CheckPacketCasting<C_Login>(packet, session) == false)
                return;
            var c_packet = packet as C_Login;

            var user = Gatekeeper.Inst.TryLogin(c_packet.ID, c_packet.PW);
            var c_session = session as ClientSession;
            c_session.UserId = user?.Id;
            S_Login s_packet = new S_Login(user != null, user);
            session.RegisterSend(PacketMgr.Inst.PacketToByte(s_packet));
        }
        void HandleLogout(IPacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling Logout packet from session {session.SessionID}");
            if (CheckPacketCasting<C_Logout>(packet, session) == false)
                return;
            var c_packet = packet as C_Logout;
            S_Logout s_packet;
            if (Gatekeeper.Inst.TryLogout(c_packet.User.Id) == false)
                return;
            s_packet = new S_Logout(true);
            session.RegisterSend(s_packet);
        }
        void HandleEnterLobby(IPacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling EnterLobby packet from session {session.SessionID}");
            if (CheckPacketCasting<C_EnterLobby>(packet, session) == false)
                return;
            var c_packet = packet as C_EnterLobby;
            var user = c_packet.User;
            if (user == null || user?.UserState == Define.User_State.lobby)
            {
                _ts.TraceEvent(TraceEventType.Warning, 1, $"[PacketHandler] Invalid userInfo from session {session.SessionID}");
                return;
            }
            var original = Gatekeeper.Inst.EnterLobby(user.Id);    
            S_EnterLobby s_packet = new S_EnterLobby(original != null, original);
            session.RegisterSend(PacketMgr.Inst.PacketToByte(s_packet));
        }
        void HandleUpdateLobby(IPacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling UpdateLobby packet from session {session.SessionID}");
            if (CheckPacketCasting<C_UpdateLobby>(packet, session) == false)
                return;
            var c_packet = packet as C_UpdateLobby;
            S_UpdateLobby s_packet;
            if (Concierge.Inst.CheckUser(c_packet.User) == false)
            {
                //error
                s_packet = new S_UpdateLobby(null);
                s_packet.Message = "User check failed";
                session.RegisterSend(s_packet);
                return;
            }
            LobbyInfo info = Concierge.Inst.GetLobbyInfo();
            s_packet = new S_UpdateLobby(info);
            session.RegisterSend(s_packet);
        }
        void HandleEnterRoom(IPacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling EnterRoom packet from session {session.SessionID}");
            if (CheckPacketCasting<C_EnterRoom>(packet, session) == false)
                return;
            S_EnterRoom s_packet;
            var c_packet = packet as C_EnterRoom;
            s_packet = new S_EnterRoom();
            if (Concierge.Inst.Check_ReserveRoom(session, c_packet.User,c_packet.RoomId, ref s_packet.Message) == false)
            {
                s_packet.Answer = false;
                s_packet.User = c_packet.User; // if user != null : simple error, if null : fatal error
                session.RegisterSend(s_packet);
                return;
            }
            s_packet.User = Concierge.Inst.GuideRoom(c_packet.User, c_packet.RoomId);
            s_packet.Answer = true;
            session.RegisterSend(s_packet);

        }
        void HandleExitRoom(IPacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling ExitRoom packet from session {session.SessionID}");
            if (CheckPacketCasting<C_ExitRoom>(packet, session) == false)
                return;
            var c_packet = packet as C_ExitRoom;
            S_ExitRoom s_packet;
            if (Concierge.Inst.CheckUser(c_packet.User) == false)
            {
                //error
                s_packet = new S_ExitRoom(null,false);
                s_packet.Message = "User check failed";
                session.RegisterSend(s_packet);
                return;
            }
            var original = Concierge.Inst.GuideLobby(c_packet.User);

            s_packet = new S_ExitRoom(original, true);
            session.RegisterSend(s_packet);
        }
        void HandleUpdateRoom(IPacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling UpdateRoom packet from session {session.SessionID}");
            if (CheckPacketCasting<C_UpdateRoom>(packet, session) == false)
                return;
            var c_packet = packet as C_UpdateRoom;
            S_UpdateRoom s_packet;
            if (Concierge.Inst.CheckUser(c_packet.User) == false)
            {
                //error
                s_packet = new S_UpdateRoom(null);
                s_packet.Message = "You are not checked-in";
                session.RegisterSend(s_packet);
                return;
            }
            var chatting = Concierge.Inst.GetChatting(c_packet.RoomId);
            s_packet = new S_UpdateRoom(chatting);
            session.RegisterSend(s_packet);
        }
        void HandleChat(IPacket packet, Session session)
        {
            _ts.TraceInformation($"[PacketHandler] Handling Chat packet from session {session.SessionID}");
            if (CheckPacketCasting<C_Chat>(packet, session) == false)
                return;
            var c_packet = packet as C_Chat;
            S_UpdateRoom s_packet;
            if (Concierge.Inst.CheckUser(c_packet.Chat.User) == false)
            {
                //error
                s_packet = new S_UpdateRoom(null);
                s_packet.Message = "You are not checked-in";
                session.RegisterSend(s_packet);
                return;
            }
            var chatting = Concierge.Inst.PushChat(c_packet.Chat.User.RoomId ,c_packet.Chat);
            s_packet = new S_UpdateRoom(chatting);
            session.RegisterSend(s_packet);

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

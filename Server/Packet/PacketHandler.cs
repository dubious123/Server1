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

namespace Server
{
    public class PacketHandler
    {
        static PacketHandler _instance = new PacketHandler();
        public static PacketHandler Inst { get { return _instance; } }
        PacketHandler()
        {
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
            Action<IPacket, Session> action;
            _handlerDict.TryGetValue(packet.PacketId, out action);
            action?.Invoke(packet, session);
        }
        void HandleLogin(IPacket packet, Session session)
        {
            var c_packet = packet as C_Login;

            var user = Gatekeeper.Inst.TryLogin(c_packet.ID, c_packet.PW);   
            S_Login s_packet = new S_Login(user != null, user);
            session.RegisterSend(PacketMgr.Inst.PacketToByte(s_packet));
        }
        void HandleLogout(IPacket packet, Session session)
        {
            var c_packet = packet as C_Logout;
            S_Logout s_packet;
            if (Concierge.Inst.CheckUser(c_packet.User) == false)
            {
                s_packet = new S_Logout(false, "failed to check user");
                session.RegisterSend(s_packet);
            }
            Gatekeeper.Inst.TryLogout(c_packet.User.Id);
            s_packet = new S_Logout(true);
            session.RegisterSend(s_packet);
        }
        void HandleEnterLobby(IPacket packet, Session session)
        {
            var c_packet = packet as C_EnterLobby;
            var user = c_packet.User;
            user = Gatekeeper.Inst.EnterLobby(user.Id);
            if (user?.UserState != Define.User_State.lobby)
                throw new Exception();
            S_EnterLobby s_packet = new S_EnterLobby(user != null, user);
            session.RegisterSend(PacketMgr.Inst.PacketToByte(s_packet));
        }
        void HandleUpdateLobby(IPacket packet, Session session)
        {
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
            S_EnterRoom s_packet;
            var c_packet = packet as C_EnterRoom;
            s_packet = new S_EnterRoom();
            if (Concierge.Inst.Check_ReserveRoom(session, c_packet.User,c_packet.RoomId, ref s_packet.Message) == false)
            {
                s_packet.Answer = false;
                session.RegisterSend(s_packet);
                return;
            }
            Concierge.Inst.GuideRoom(c_packet.User, c_packet.RoomId);
            Concierge.Inst.UpdateUserInfo(c_packet.User);
            s_packet.Answer = true;
            s_packet.User = c_packet.User;
            session.RegisterSend(s_packet);

        }
        void HandleExitRoom(IPacket packet, Session session)
        {
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
            Concierge.Inst.GuideLobby(c_packet.User);
            Concierge.Inst.UpdateUserInfo(c_packet.User);

            s_packet = new S_ExitRoom(c_packet.User,true);
            session.RegisterSend(s_packet);
        }
        void HandleUpdateRoom(IPacket packet, Session session)
        {          
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
    }
}

using PacketTools;
using ServerCore;
using ServerCore.Packets;
using ServerCore.Packets.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Me
{
    public class PacketHandler
    {
        static PacketHandler _instance = new PacketHandler();
        public static PacketHandler Inst { get { return _instance; } }
        PacketHandler()
        {
            _handlerDict = new Dictionary<ushort, Action<IPacket, Session>>();
            _handlerDict.Add((ushort)Define.P_Id.s_login, HandleLogin);
            _handlerDict.Add((ushort)Define.P_Id.s_logout, HandleLogout);
            _handlerDict.Add((ushort)Define.P_Id.s_enterLobby, HandleEnterLobby);
            _handlerDict.Add((ushort)Define.P_Id.s_updateLobby, HandleUpdateLobby);
            _handlerDict.Add((ushort)Define.P_Id.s_enterRoom, HandleEnterRoom);
            _handlerDict.Add((ushort)Define.P_Id.s_exitRoom, HandleExitRoom);
            _handlerDict.Add((ushort)Define.P_Id.s_updateRoom, HandleUpdateRoom);
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
            var s_packet = packet as S_UpdateLobby;
            ContentMgr.Inst.UpdateLobby(s_packet.Info);
            
            CmdMgr.Inst.UpdateState(Define.Cmd_State.done);
        }
        void HandleEnterRoom(IPacket packet, Session session)
        {
            var s_packet = packet as S_EnterRoom;
            if(s_packet.Answer == false)
            {
                CmdMgr.Inst.UpdateState(Define.Cmd_State.error, s_packet.Message);
                return;
            }
            ContentMgr.Inst.UpdateUserInfo(s_packet.User);
            CmdMgr.Inst.UpdateState(Define.Cmd_State.done);

        }
        void HandleExitRoom(IPacket packet, Session session)
        {
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
            var s_packet = packet as S_UpdateRoom;
            ContentMgr.Inst.ShowChats(s_packet.Chats);
            CmdMgr.Inst.UpdateState(Define.Cmd_State.done);
        }
    }
}

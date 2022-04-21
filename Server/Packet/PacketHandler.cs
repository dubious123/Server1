﻿using PacketTools;
using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class PacketHandler
    {
        static PacketHandler _instance = new PacketHandler();
        public static PacketHandler Inst { get { return _instance; } }
        PacketHandler()
        {
            _handlerDict = new Dictionary<ushort, Action<IPacket, Session>>();
            _handlerDict.Add((ushort)Define.PacketId.c_login, HandleLogin);

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
            var c_packet = packet as C_LoginPacket;

            var answer = Gatekeeper.Inst.TryLogin(c_packet.ID, c_packet.PW);
            S_LoginPacket s_packet = new S_LoginPacket(answer);
            session.RegisterSend(PacketMgr.Inst.PacketToByte(s_packet));
        }

    }
}

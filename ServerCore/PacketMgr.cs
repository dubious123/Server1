using PacketTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore.Packets;
using ServerCore.Packets.Client;
using ServerCore.Packets.Server;
using ServerCore.Log;
using System.Diagnostics;

namespace ServerCore
{
    public class PacketMgr
    {
        static PacketMgr _instance = new PacketMgr();
        public static PacketMgr Inst { get { return _instance; } }
        static Dictionary<ushort, Func<byte[], IPacket>> _readDict;


        PacketMgr()
        {

            
            _readDict = new Dictionary<ushort, Func<byte[], IPacket>>();
            _readDict.Add((ushort)Define.P_Id.c_login, BuildPacket<C_Login>);
            _readDict.Add((ushort)Define.P_Id.c_logout, BuildPacket<C_Logout>);
            _readDict.Add((ushort)Define.P_Id.c_enterLobby, BuildPacket<C_EnterLobby>);
            _readDict.Add((ushort)Define.P_Id.c_updateLobby, BuildPacket<C_UpdateLobby>);
            _readDict.Add((ushort)Define.P_Id.c_enterRoom, BuildPacket<C_EnterRoom>);
            _readDict.Add((ushort)Define.P_Id.c_exitRoom, BuildPacket<C_ExitRoom>);
            _readDict.Add((ushort)Define.P_Id.c_updateRoom, BuildPacket<C_UpdateRoom>);
            _readDict.Add((ushort)Define.P_Id.c_chat, BuildPacket<C_Chat>);


            _readDict.Add((ushort)Define.P_Id.s_login, BuildPacket<S_Login>);
            _readDict.Add((ushort)Define.P_Id.s_logout, BuildPacket<S_Logout>);
            _readDict.Add((ushort)Define.P_Id.s_enterLobby, BuildPacket<S_EnterLobby>);
            _readDict.Add((ushort)Define.P_Id.s_updateLobby, BuildPacket<S_UpdateLobby>);
            _readDict.Add((ushort)Define.P_Id.s_enterRoom, BuildPacket<S_EnterRoom>);
            _readDict.Add((ushort)Define.P_Id.s_exitRoom, BuildPacket<S_ExitRoom>);
            _readDict.Add((ushort)Define.P_Id.s_updateRoom, BuildPacket<S_UpdateRoom>);
        }

        public List<IPacket> ByteToPacket(RecvBuffer buffer)
        {
            List<IPacket> list = new List<IPacket>();
            Func<byte[], IPacket> func;
            while (buffer.CanRead())
            {
                var packetSize = BitConverter.ToUInt16(buffer.Read(2)) - 4;
                var packetId = BitConverter.ToUInt16(buffer.Read(2));
                if (packetSize <= 0)
                    break;
                if (!_readDict.TryGetValue(packetId, out func))
                    return null;
                var json = buffer.Read(packetSize).ToArray();
                list.Add(func.Invoke(json));
            }
            return list;
        }
        
        T BuildPacket<T>(byte[] json) where T : IPacket
        {
            return PacketSerializer.DeSerialize_Json<T>(json);
        }



        public byte[] PacketToByte<T>(T packet) where T : IPacket
        {           
            var json = PacketSerializer.Serialize_Json<T>(packet);
            return AttachHeader((ushort)(json.Length + 4), packet.PacketId, json);
        }
        public void PacketToByte<T>(T packet, out byte[] arr) where T : IPacket
        {
            var json = PacketSerializer.Serialize_Json<T>(packet);
            arr = AttachHeader((ushort)(json.Length + 4), packet.PacketId, json);
            if(arr == null)
            {
                throw new Exception();
            }
        }
        byte[] AttachHeader(ushort size, ushort id, byte[] json)
        {
            byte[] serialized = new byte[size];
            Array.Copy(BitConverter.GetBytes(size), 0, serialized, 0, 2);
            Array.Copy(BitConverter.GetBytes(id), 0, serialized, 2, 2);
            Array.Copy(json, 0, serialized, 4, json.Length);
            return serialized;
        }

    }
}

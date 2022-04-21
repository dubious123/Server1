using PacketTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _readDict.Add((ushort)Define.PacketId.c_login, BuildPacket<C_LoginPacket>);
            _readDict.Add((ushort)Define.PacketId.s_login, BuildPacket<S_LoginPacket>);
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

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketTools
{
    public class PacketSerializer
    {
        public static byte[] Serialize_Json<T>(T packet) where T : IPacket
        {          
            return Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(packet));
        }
        public static T DeSerialize_Json<T>(byte[] data) where T : class, new()
        {
            return JsonConvert.DeserializeObject<T>(Encoding.Unicode.GetString(data));
        }
    }
}

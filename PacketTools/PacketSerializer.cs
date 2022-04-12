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
        public static byte[] Serialize_Json(object clss)
        {
            LinkedList<byte[]> data = new LinkedList<byte[]>();
            var temp = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(clss));
            var size = (ushort)temp.Length + 2;
            byte[] serialized = new byte[size];
            Array.Copy(BitConverter.GetBytes(size), serialized, 2);
            Array.Copy(temp, 0, serialized, 2, temp.Length);
            return serialized;
        }
        public static T DeSerialize_Json<T>(byte[] data) where T : class
        {            
            return JsonConvert.DeserializeObject(BitConverter.ToString(data)) as T;
        }
    }
}

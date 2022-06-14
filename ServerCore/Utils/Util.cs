using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Util
    {
        public static Tuple<int, int, int> Get_Woker_Completion_TotalThreadNumTuple()
        {
            ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int maxCompletionPortThreads);
            ThreadPool.GetAvailableThreads(out int workerThreads, out int completionPortThreads);
            //Console.WriteLine(
            //    "Worker threads: {0}, Completion port threads: {1}, Total threads: {2}",
            //    maxWorkerThreads - workerThreads,
            //    maxCompletionPortThreads - completionPortThreads,
            //    Process.GetCurrentProcess().Threads.Count
            //);
            return new Tuple<int, int, int>(maxWorkerThreads - workerThreads,
                maxCompletionPortThreads - completionPortThreads,
                Process.GetCurrentProcess().Threads.Count);
        }
        public static byte[] Serialize_Json<T>(T packet) where T : BasePacket
        {
            return Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(packet));
        }
        public static T DeSerialize_Json<T>(byte[] data) where T : BasePacket
        {
            return JsonConvert.DeserializeObject<T>(Encoding.Unicode.GetString(data));
        }
    }
}

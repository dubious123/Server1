using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ServerCore
{
    public static class Extention
    {
        public static void TraceInfo(this TraceSource ts, string message)
        {
            ts.TraceInformation(message);
        }

    }
}

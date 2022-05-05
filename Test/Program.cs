// The following configuration file can be used with this sample.
// When using a configuration file #define ConfigFile.
//            <source name="TraceTest" switchName="SourceSwitch" switchType="System.Diagnostics.SourceSwitch" >
//                    <add name="console" type="System.Diagnostics.ConsoleTraceListener" initializeData="false" />
//                    <remove name ="Default" />
//            <!-- You can set the level at which tracing is to occur -->
//            <add name="SourceSwitch" value="Warning" />
//            <!-- You can turn tracing off -->
//            <!--add name="SourceSwitch" value="Off" -->
//        <trace autoflush="true" indentsize="4"></trace>
#define TRACE
//#define ConfigFile

using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.IO;
using System.Security.Permissions;

namespace Testing
{
    class TraceTest
    {
        struct SimpleStruct
        {
            public long num;
            public SimpleStruct(long n)
            {
                num = n;
            }
        }

        static void Main()
        {
            SimpleStruct st = new SimpleStruct(10000000000);
            long tick = Environment.TickCount64;
            Console.WriteLine("Pass by Value Started");
            PassByValue(st);
            Console.WriteLine("Pass by Value End");
            Console.WriteLine($"ExTick : {Environment.TickCount64 - tick}");
            tick = Environment.TickCount64;
            Console.WriteLine("Pass by Ref Started");
            PassByRef(ref st);
            Console.WriteLine("Pass by Ref End");
            Console.WriteLine($"ExTick : {Environment.TickCount64 - tick}");


        }

        static void PassByRef(ref SimpleStruct st)
        {
            for (int i = 0; i < st.num; i++)
                st.num--;
        }
        static void PassByValue(SimpleStruct st)
        {
            for (int i = 0; i < st.num; i++)
                st.num--;
        } 
    }
}
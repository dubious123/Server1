using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;

namespace ServerCore.Log
{
    public class LogMgr
    {
        static string _dirPath = Directory.GetCurrentDirectory() + "/../../../../Logs/";
        static ConcurrentDictionary<string, TraceSource> _sourceDict = new ConcurrentDictionary<string, TraceSource>();
        static public void ChooseSaveDir(string dirName)
        {
            _dirPath += dirName;
        }
        static public TraceSource AddNewSource(string name, SourceLevels levels)
        {
            Trace.AutoFlush = true;
            var ts = new TraceSource(name, levels);
            ts.Listeners.Remove("Default");
            ts.Switch = new SourceSwitch("Server Switch", "information");

            _sourceDict.TryAdd(name, ts);
            return ts;
        }
        static public TraceSource GetTraceSource(string stName)
        {
            return _sourceDict[stName];
        }
        static public int AddNewConsoleListener(string tsName, string name, bool send2error, 
            SourceLevels level = SourceLevels.Information, params TraceOptions[] options)
        {
            var c_listener = new ConsoleTraceListener(send2error);
            c_listener.Name = name;
            
            
            return AddNewListener(tsName, c_listener, level,  options);
        }
        static public int AddNewTextWriterListener(string tsName, string name, string fileName,
            SourceLevels level = SourceLevels.Information,params TraceOptions[] options)
        {
            FileStream hlogFile = new FileStream(_dirPath + $"/{fileName}", FileMode.OpenOrCreate, FileAccess.Write);
            return AddNewListener(tsName, new TextWriterTraceListener(hlogFile, name), level, options);
        }
        static public int AddNewListener(string tsName, TraceListener listener, 
            SourceLevels level = SourceLevels.Information, params TraceOptions[] options)
        {
            listener.Filter = new EventTypeFilter(level);
            foreach (var option in options)
                listener.TraceOutputOptions |= option;
            return _sourceDict[tsName].Listeners.Add(listener);
        }
        static public void UpdateListenerFilter(string tsName, int index, SourceLevels level)
        {
            _sourceDict[tsName].Listeners[index].Filter = new EventTypeFilter(level);
        }
        static public SourceSwitch GetSwitch(string tsName)
        {
            return _sourceDict[tsName].Switch;
        }
        static public TraceListener GetListener(string tsName, int ListenerIndex)
        {
            return _sourceDict[tsName].Listeners[ListenerIndex];
        }
        static public void TraceInfo(string tsName, string message)
        {
            _sourceDict[tsName].TraceInformation(message);
        }
        static public void TraceEvent(string tsName, TraceEventType type, int id, string message)
        {
            _sourceDict[tsName].TraceEvent(type, id, message);
        }
    }
}

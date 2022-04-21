using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Gatekeeper
    {
        static Gatekeeper _inst = new Gatekeeper();
        public static Gatekeeper Inst { get { return _inst; } }
        ConcurrentDictionary<string, string> _userDict;
        Gatekeeper()
        {
            _userDict = new ConcurrentDictionary<string, string>();
            _userDict.TryAdd("jonghun", "990827");
        }
        public bool TryLogin(string id, string ps)
        {
            if (_userDict.TryGetValue(id, out string password))
                return password == ps;
            return false;
            
        }

    }
}

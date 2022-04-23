using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class UserData
    {
        public string Name;
        public string Id;
        public string Pw;
        public UserData(string name, string id, string pw)
        {
            Name = name;
            Id = id;
            Pw = pw;
        }
    }
}

using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    public class DummyMgr
    {
        static DummyMgr _inst = new DummyMgr();
        public static DummyMgr Inst { get { return _inst; } }
        Dummy[] _dummyArr;
        public void Init(int dummyNum)
        {
            _dummyArr = new Dummy[dummyNum];
            for (uint i = 0; i < dummyNum; i++)    
                _dummyArr[i] = new Dummy(SessionMgr.Inst.Find(i + 1));
        }
        public void ControlDummies()
        {
            foreach(var d in _dummyArr)
                d.Act();
            JobMgr.Inst.Push("Dummy", ControlDummies);
        }
        public Dummy GetDummies(uint i)
        {
            return _dummyArr[i - 1];
        }
    }
}

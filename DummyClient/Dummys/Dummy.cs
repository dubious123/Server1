using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ServerCore.Log;
using static ServerCore.Define;
using ServerCore.Packets.Client;
using ServerCore.Packets;

namespace DummyClient
{
    public class Dummy
    {
        UserInfo _user;
        Session _session;
        int[] _cmdList;
        int _cmdIndex;
        Random _rand;
        TraceSource _ts;
        Cmd_State _state;
        LobbyInfo _lobby;
        string _message;
        const string _chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZasbcdfghijklmnopqrstuvwxyz0123456789           ";
        public Dummy(Session session)
        {
            _ts = LogMgr.GetTraceSource("Dummy");
            _user = new UserInfo($"User_{session.SessionID}",$"Dummy_{session.SessionID}");
            _session = session;
            _cmdIndex = 0;
            _cmdList = new int[500];
            _rand = new Random(DateTime.Now.Millisecond + (int)session.SessionID);
            FillCmdList();
            _state = Cmd_State.start;
        }
        public void UpdateState(Cmd_State state, string message = null)
        {
            _state = state;
            _message = message;
        }
        public void FillCmdList()
        {
            for (int i = 0; i < _cmdList.Length; i++)
                _cmdList[i] = _rand.Next();
            _cmdIndex = 0;
        }
        int GetNextRand()
        {
            if (_cmdIndex >= _cmdList.Length)
                FillCmdList();
            return _cmdList[_cmdIndex++];
        }
        public void Act()
        {
            if (_state == Cmd_State.wait)
                return;
            if (_state == Cmd_State.error)
            {
                HandleError();
                return;
            }
            var cmd = GetNextRand() % 10;
            if (cmd < 1)
            {
                DoNothing();
                return;
            }
            
            switch (_user.UserState)
            {
                case User_State.logout:
                    Login();
                    break;
                case User_State.login:
                    if (cmd < 2)
                        Logout();
                    else
                        EnterLobby();
                    break;
                case User_State.lobby:
                    if (cmd < 2)
                        Logout();
                    else if (cmd < 5)
                        UpdateLobby();
                    else
                        EnterRoom();
                    break;
                case User_State.room:
                    if (cmd < 2)
                        Logout();
                    else if (cmd < 4)
                        UpdateRoom();
                    else if (cmd < 9)
                        Chat();
                    else
                        ExitRoom();
                    break;
                default:
                    break;
            }
            _state = Cmd_State.wait;
        }
        public void UpdateUserInfo(UserInfo user)
        {
            if (user != null)
            {
                _ts.TraceInfo($"[{_user.Name}] Updating UserInfo \n" +
                              $"before : State[{_user.UserState}] Room Id[{_user.RoomId}] \n" +
                              $"after : State[{user.UserState}] Room Id[{user.RoomId}]");
            }
            else
                _ts.TraceEvent(TraceEventType.Warning, 1, $"[{_user.Name}] Updating user to null");

            _user = user;
        }
        public void UpdateLobby(LobbyInfo lobby)
        {
            string traceInfo = $"[{_user.Name}] Updating LobbyInfo \n Before : \n";
            if(_lobby != null)
                foreach (var t in _lobby.RoomInfoDict)
                    traceInfo += $"Room[{t.Key}] {t.Value.CurrentUserNum}/{t.Value.MaxUserNum} \n";
            else
                traceInfo += $"null \n";

            traceInfo += "After : \n";
            foreach (var t in lobby.RoomInfoDict)
                traceInfo += $"Room[{t.Key}] {t.Value.CurrentUserNum}/{t.Value.MaxUserNum} \n";
            _lobby = lobby;
        }
        uint GetRandomRoomId()
        {
            var keys = _lobby.RoomInfoDict.Keys.ToArray();
            int length = keys.Length;
            return keys[GetNextRand() % length];
        }
        string GetRandomChat()
        {         
            return new string(Enumerable.Repeat(_chars, GetNextRand() % 20)
                .Select(s => s[GetNextRand() % _chars.Length]).ToArray());
        }
        void DoNothing()
        {
            _ts.TraceInfo($"[{_user.Name}] DoNothing");
        }
        void Login()
        {
            _ts.TraceInfo($"[{_user.Name}] Login");
            var c_packet = new C_Login(_user.Id, $"User_{_session.SessionID * 100}");
            _session.RegisterSend(c_packet);
        }

        void Logout()
        {
            _ts.TraceInfo($"[{_user.Name}] Logout");
            var c_packet = new C_Logout(_user);
            _session.RegisterSend(c_packet);
        }
        void EnterLobby()
        {
            _ts.TraceInfo($"[{_user.Name}] EnterLobby");
            var c_packet = new C_EnterLobby(_user);
            _session.RegisterSend(c_packet);
        }
        void UpdateLobby()
        {
            _ts.TraceInfo($"[{_user.Name}] UpdateLobby");
            var c_packet = new C_UpdateLobby(_user);
            _session.RegisterSend(c_packet);
        }
        void EnterRoom()
        {
            if(_lobby == null)
            {
                UpdateLobby();
                return;
            }
            _ts.TraceInfo($"[{_user.Name}] EnterRoom");
            var c_packet = new C_EnterRoom(_user, GetRandomRoomId());
            _session.RegisterSend(c_packet);
        }
        void ExitRoom()
        {
            _ts.TraceInfo($"[{_user.Name}] ExitRoom");
            var c_packet = new C_ExitRoom(_user);
            _session.RegisterSend(c_packet);

        }
        void Chat()
        {
            var str = GetRandomChat();
            _ts.TraceInfo($"[{_user.Name}] Chat : " + str);
            Chatting chat = new Chatting(_user, str);
            var c_packet = new C_Chat(chat);
            _session.RegisterSend(c_packet);
        }
        void UpdateRoom()
        {
            _ts.TraceInfo($"[{_user.Name}] UpdateRoom");
            var c_packet = new C_UpdateRoom(_user);
            _session.RegisterSend(c_packet);
        }
        void HandleError()
        {
            _ts.TraceEvent(TraceEventType.Error, 0, $"[{_user.Name}] Handling error " + _message);
            ResetDummy();
        }
        public void ResetDummy()
        {
            _ts.TraceInfo($"[{_user.Name}] Reseting ");
            _user = new UserInfo($"User_{_session.SessionID}", $"Dummy_{_session.SessionID}");
            FillCmdList();
            _state = Cmd_State.start;
        }

    }
}

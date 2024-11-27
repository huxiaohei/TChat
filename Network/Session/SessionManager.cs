/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 11:42:27
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using System.Collections.Concurrent;
using TChat.Abstractions.Network;

namespace Tchat.Network.Session
{
    public class SessionManager : ISessionManager
    {
        private readonly ConcurrentDictionary<long, ISession> _sessions = new ConcurrentDictionary<long, ISession>();

        public void AddSession(ISession session)
        {
            _sessions.TryAdd(session.SessionId, session);
        }

        public void RemoveSession(long sessionId)
        {
            _sessions.TryRemove(sessionId, out _);
        }

        public ISession? GetSession(long sessionId)
        {
            _sessions.TryGetValue(sessionId, out var session);
            return session;
        }
    }
}
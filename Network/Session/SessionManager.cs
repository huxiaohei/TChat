﻿/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 11:42:27
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Network;
using System.Collections.Concurrent;
using Utils.LoggerUtil;

namespace Network.Session
{
    public class SessionManager : ISessionManager
    {
        private readonly ConcurrentDictionary<long, ISession> _sessions = new();

        public SessionManager()
        {
            Loggers.Network.Info("SessionManager is ready.");
        }

        public void AddSession(ISession session)
        {
            _sessions.TryAdd(session.SessionId, session);
        }

        public void RemoveSession(long sessionId)
        {
            if (_sessions.TryRemove(sessionId, out var _))
            {
                Loggers.Network.Info($"Session {sessionId} removed.");
            }
        }

        public ISession? GetSession(long sessionId)
        {
            _sessions.TryGetValue(sessionId, out var session);
            return session;
        }
    }
}
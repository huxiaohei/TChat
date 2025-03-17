/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 11:55:46
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Orleans.Concurrency;
using Abstractions.Grains;
using Abstractions.Message;
using Abstractions.Network;
using Utils.LoggerUtil;

namespace ChatServer.Grains
{
    [Reentrant]
    public class BaseGrainService(GrainId grainId, Silo silo, ILoggerFactory loggerFactory, ISessionManager sessionManager)
        : GrainService(grainId, silo, loggerFactory), IBaseGrainService
    {
        private readonly ISessionManager _sessionManager = sessionManager;

        public async Task SendMessageAsync(long sessionId, ISCMessage message)
        {
            var session = _sessionManager.GetSession(sessionId);
            if (session != null)
            {
                await session.SendMessageAsync(message);
            }
            else
            {
                Loggers.Chat.Info($"Session {sessionId} not found");
            }
        }

        public async Task CloseSessionAsync(long sessionId)
        {
            var session = _sessionManager.GetSession(sessionId);
            if (session != null)
            {
                await session.CloseAsync();
                _sessionManager.RemoveSession(sessionId);
            }
            else
            {
                Loggers.Chat.Info($"Session {sessionId} not found");
            }
        }
    }
}
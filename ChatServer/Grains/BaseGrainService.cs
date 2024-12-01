/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 11:55:46
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Orleans.Concurrency;
using TChat.Abstractions.Grains;
using TChat.Abstractions.Message;
using TChat.Abstractions.Network;

namespace TChat.ChatServer.Grains
{

    [Reentrant]
    public class BaseGrainService : GrainService, IBaseGrainService
    {
        private ISessionManager _sessionManager { get; }

        public BaseGrainService(GrainId grainId, Silo silo, ILoggerFactory loggerFactory, ISessionManager sessionManager)
            : base(grainId, silo, loggerFactory)
        {
            _sessionManager = sessionManager;
        }

        public async Task SendMessageAsync(long sessionId, ISCMessage message)
        {
            var session = _sessionManager.GetSession(sessionId);
            if (session != null)
            {
                await session.SendMessageAsync(message);
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
        }

    }
}
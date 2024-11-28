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
        private ISessionManager SessionManager { get; }

        public BaseGrainService(GrainId grainId, Silo silo, ILoggerFactory loggerFactory, ISessionManager sessionManager)
            : base(grainId, silo, loggerFactory)
        {
            SessionManager = sessionManager;
        }

        public Task SendMessageAsync(long sessionId, ISCMessage message)
        {
            throw new NotImplementedException();
        }

        public Task CloseSessionAsync(long sessionId)
        {
            throw new NotImplementedException();
        }
    }

}
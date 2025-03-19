/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 18:03:31
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Grains;
using Abstractions.Message;

namespace ChatServer.Message
{
    public class ChatMessageHandler(IGrainFactory factory, SiloAddress siloAddress) : IMessageHandler
    {
        public async Task<ISCMessage?> HandleMessageAsync(long sessionId, ICSMessage message)
        {
            return await factory.GetGrain<IPlayerGrain>(message.RoleId).ProcessMessageAsync(siloAddress, sessionId, message);
        }
    }
}
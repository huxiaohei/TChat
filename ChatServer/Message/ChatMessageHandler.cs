/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 18:03:31
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using TChat.Abstractions.Grains;
using TChat.Abstractions.Message;

namespace TChat.ChatServer.Message
{
    public class ChatMessageHandler(SiloAddress siloAddress, IClusterClient clusterClient) : IMessageHandler
    {
        public async Task<ISCMessage?> HandleMessage(long sessionId, ICSMessage message)
        {
            return await clusterClient.GetGrain<IPlayerGrain>(message.RoleId).ProcessMessage(siloAddress, sessionId, message);
        }
    }

}
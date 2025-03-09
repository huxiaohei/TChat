/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 18:03:31
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Grains;
using Abstractions.Message;
using Google.Protobuf;

namespace ChatServer.Message
{
    public class ChatMessageHandler() : MessageHandler
    {
        public override async Task<(uint, IMessage?)> HandleMessage(long sessionId, ICSMessage message)
        {
            return await ClusterClient.GetGrain<IPlayerGrain>(message.RoleId).ProcessMessage(SiloAddress, sessionId, message);
        }
    }
}
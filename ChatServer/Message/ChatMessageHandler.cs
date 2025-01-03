/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 18:03:31
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Google.Protobuf;
using TChat.Abstractions.Grains;
using TChat.Abstractions.Message;

namespace TChat.ChatServer.Message
{
    public class ChatMessageHandler() : MessageHandler
    {
        override public async Task<(uint, IMessage?)> HandleMessage(long sessionId, ICSMessage message)
        {
            return await ClusterClient.GetGrain<IPlayerGrain>(message.RoleId).ProcessMessage(SiloAddress, sessionId, message);
        }
    }

}
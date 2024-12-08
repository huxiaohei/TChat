/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/12/1 16:07:27
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Google.Protobuf;
using TChat.Network.Protos;
using TChat.Abstractions.Message;
using TChat.ChatServer.Extensions;


namespace TChat.ChatServer.Message
{
    public class FirstMessageHandler : MessageHandler
    {
        override public async Task<(uint, IMessage?)> HandleMessage(long _, ICSMessage message)
        {
            if (message.Message is CSLoginReq)
            {
                var msg = message.Cast<CSLoginReq>();
                if (string.IsNullOrEmpty(msg.Token))
                {
                    return
                }

            }
            if (message.Message is CSReLoginReq)
            {
                // TODO: Check Token
            }
            return default;
        }
    }

}
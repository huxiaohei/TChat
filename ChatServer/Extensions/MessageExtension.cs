/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/12/2 23:45:55
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Google.Protobuf;
using Abstractions.Message;

namespace ChatServer.Extensions
{
    public static class MessageExtension
    {
        public static T Cast<T>(this ICSMessage message) where T : class, IMessage
        {
            if (message.Message is not T result)
            {
                throw new InvalidCastException("Failed to cast message to the specified type.");
            }
            return result;
        }
    }
}
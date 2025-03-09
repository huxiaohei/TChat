/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/12/2 23:45:55
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Google.Protobuf;
using Abstractions.Message;
using Network.Protos;

namespace ChatServer.Extensions
{

    public static class MessageExtension
    {
        public static IMessage Msg(this ErrCode code)
        {
            return new SCErrResp()
            {
                ErrCode = code,
                ErrMsg = ""
            };
        }

        public static T Cast<T>(this ICSMessage message) where T : class, IMessage
        {
            var result = message.Message as T;
            if (result == null)
            {
                throw new InvalidCastException("Failed to cast message to the specified type.");
            }
            return result;
        }
    }

}
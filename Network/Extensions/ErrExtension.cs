/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 15/03/2025, 23:41:50
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Google.Protobuf;
using Network.Protos;

namespace Network.Extensions
{
    public static class ErrExtension
    {
        public static IMessage Msg(this ErrCode code)
        {
            return new SCErrResp()
            {
                ErrCode = code,
                ErrMsg = code.ToString()
            };
        }
    }

}
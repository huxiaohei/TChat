/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/27 15:04:36
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Google.Protobuf;

namespace Abstractions.Message
{
    public interface ISCMessage
    {
        long RoleId { get; }
        bool IsCache { get; set; }
        uint ClientSerialId { get; }
        uint ServerSerialId { get; }
        string MsgName { get; }
        IMessage Message { get; }

        byte[] Encode();
    }
}
/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 17:40:01
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

namespace Abstractions.Message
{
    public interface IMessageHandler
    {
        Task<ISCMessage?> HandleMessageAsync(long sessionId, ICSMessage message);
    }
}
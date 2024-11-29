/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/27 14:59:55
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Orleans.Services;
using TChat.Abstractions.Message;

namespace TChat.Abstractions.Grains
{
    public interface IBaseGrainService : IGrainService
    {
        Task SendMessageAsync(long sessionId, [Immutable] ISCMessage message);
        Task CloseSessionAsync(long sessionId);
    }
}
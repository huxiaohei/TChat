/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/27 14:59:55
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Orleans.Services;
using Abstractions.Message;

namespace Abstractions.Grains
{
    [Alias("Abstractions.Grains.IBaseGrainService")]
    public interface IBaseGrainService : IGrainService
    {
        [Alias("SendMessageAsync")]
        Task SendMessageAsync([Immutable] long sessionId, [Immutable] ISCMessage message);
        [Alias("CloseSessionAsync")]
        Task CloseSessionAsync([Immutable] long sessionId);
    }
}
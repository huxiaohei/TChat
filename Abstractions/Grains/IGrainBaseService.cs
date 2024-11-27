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
    public interface IGrainBaseService : IGrainService
    {
        Task SendMessageAsync(long roleId, IBaseMessage message);
        Task CloseSessionAsync(long sessionId);
    }
}
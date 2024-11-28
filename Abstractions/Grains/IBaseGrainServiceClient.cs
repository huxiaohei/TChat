/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/27 15:39:24
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Orleans.Services;
using TChat.Abstractions.Message;

namespace TChat.Abstractions.Grains
{
    public interface IBaseGrainServiceClient : IGrainServiceClient<IGrainService>
    {
        SiloAddress SiloAddress { get; }
        IServiceProvider Provider { get; }
        Task SendMessageAsync(SiloAddress siloAddress, long sessionId, ICSMessage message);
        Task CloseSessionAsync(SiloAddress siloAddress, long sessionId);
    }

}
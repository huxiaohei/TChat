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
        IServiceProvider Provider { get; }
        SiloAddress SiloAddress { get; }

        Task SendMessageAsync([Immutable] SiloAddress siloAddress, long sessionId, [Immutable] ISCMessage message);
        Task CloseSessionAsync([Immutable] SiloAddress siloAddress, long sessionId);
    }

}
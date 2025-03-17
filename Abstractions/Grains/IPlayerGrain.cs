/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 18:09:58
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Message;

namespace Abstractions.Grains
{
    [Alias("Abstractions.Grains.IPlayerGrain")]
    public interface IPlayerGrain : IGrainWithIntegerKey
    {
        [Alias("ProcessMessage")]
        Task<ISCMessage?> ProcessMessage([Immutable] SiloAddress siloAddress, [Immutable] long sessionId, [Immutable] ICSMessage message);
    }
}
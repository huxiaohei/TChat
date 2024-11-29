/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 18:09:58
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using TChat.Abstractions.Message;

namespace TChat.Abstractions.Grains
{
    public interface IPlayerGrain : IGrainWithIntegerKey
    {
        Task<ISCMessage?> ProcessMessage([Immutable] SiloAddress siloAddress, long sessionId, [Immutable] ICSMessage message);
    }

}
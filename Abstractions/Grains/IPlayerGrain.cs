/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 18:09:58
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Google.Protobuf;
using TChat.Abstractions.Message;

namespace TChat.Abstractions.Grains
{
    public interface IPlayerGrain : IGrainWithIntegerKey
    {
        Task<(uint, IMessage?)> ProcessMessage([Immutable] SiloAddress siloAddress, [Immutable] long sessionId, [Immutable] ICSMessage message);
    }

}
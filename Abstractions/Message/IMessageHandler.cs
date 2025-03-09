/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 17:40:01
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Google.Protobuf;

namespace Abstractions.Message
{
    public interface IMessageHandler
    {
        void Bind(IClusterClient clusterClient, SiloAddress siloAddress);

        Task<(uint, IMessage?)> HandleMessage(long sessionId, ICSMessage message);
    }
}
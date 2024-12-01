/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 17:40:01
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

namespace TChat.Abstractions.Message
{
    public interface IMessageHandler
    {
        void Bind(IClusterClient clusterClient, SiloAddress siloAddress);
        Task<ISCMessage?> HandleMessage(long sessionId, ICSMessage message);
    }
}
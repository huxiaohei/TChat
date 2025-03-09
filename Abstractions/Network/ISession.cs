/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 11:04:30
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Message;

namespace Abstractions.Network
{
    public interface ISession
    {
        long SessionId { get; }

        Task SendMessageAsync(byte[] data);

        Task SendMessageAsync(ISCMessage data);

        Task CloseAsync();
    }
}
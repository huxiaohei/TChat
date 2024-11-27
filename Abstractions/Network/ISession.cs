/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 11:04:30
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

namespace TChat.Abstractions.Network
{
    public interface ISession
    {
        long SessionId { get; }
        Task SendMessageAsync(byte[] data);
        Task Close();
    }
}
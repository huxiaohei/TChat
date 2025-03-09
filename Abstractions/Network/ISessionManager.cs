/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 11:06:00
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

namespace Abstractions.Network
{
    public interface ISessionManager
    {
        void AddSession(ISession session);
        void RemoveSession(long sessionId);
        ISession? GetSession(long sessionId);
    }
}
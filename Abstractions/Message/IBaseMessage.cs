/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/27 15:04:36
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

namespace TChat.Abstractions.Message
{
    public interface IBaseMessage
    {
        public byte[] Encode();
    }
}
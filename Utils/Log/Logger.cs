/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 15:50:47
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using NLog;

namespace Utils.Log
{
    public sealed class Loggers
    {
        public static Logger Test = LogManager.GetLogger("Test");
        public static Logger Network = LogManager.GetLogger("Network");
        public static Logger Chat = LogManager.GetLogger("Chat");
        public static Logger Player = LogManager.GetLogger("Player");

    }
}
/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 15:50:47
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using NLog;

namespace Utils.LoggerUtil
{
    public sealed class Loggers
    {
        public static Logger Test { get; } = LogManager.GetLogger("Test");
        public static Logger Network { get; } = LogManager.GetLogger("Network");
        public static Logger Chat { get; } = LogManager.GetLogger("Chat");
    }
}
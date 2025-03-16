/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 15/03/2025, 23:26:38
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

namespace Utils.TimeUtil
{
    public static class Time
    {
        private static long _millisTimeOffsetForTest = 0;

        public static long NowMilliseconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + _millisTimeOffsetForTest;
        }

        public static long NowSeconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds() + _millisTimeOffsetForTest / 1000;
        }
    }
}
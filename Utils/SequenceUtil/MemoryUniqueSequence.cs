/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 11:35:36
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

namespace Utils.SequenceUtil
{
    public static class MemoryUniqueSequence
    {
        private static long _sequence = 0;

        public static long Next()
        {
            return Interlocked.Increment(ref _sequence);
        }
    }
}
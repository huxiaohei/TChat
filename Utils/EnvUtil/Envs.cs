/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 11/22/2024, 2:27:50 PM
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using System.Diagnostics.CodeAnalysis;

namespace Utils.EnvUtil
{
    public static class EnvUtils
    {
        public static bool TryGetEnv(string key, [MaybeNullWhen(false)] out string value)
        {
            var val = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(val))
            {
                value = default;
                return false;
            }
            value = val;
            return true;
        }

        public static string? GetOrDefault(string key, string? value = default)
        {
            var val = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(val))
            {
                return value;
            }
            return val;
        }

        public static T? GetOrDefault<T>(string key, T? value = default)
        {
            var val = Environment.GetEnvironmentVariable(key);
            if (string.IsNullOrEmpty(val))
            {
                return value;
            }
            return (T)Convert.ChangeType(val, typeof(T));
        }
    }
}
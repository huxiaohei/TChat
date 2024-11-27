/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 15:50:47
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using NLog;
using TChat.Utils.Envs;
using System.Diagnostics;

namespace TChat.Utils.Log
{
    public sealed class Loggers
    {
        public static Logger Test = LogManager.GetLogger("Test");
        public static Logger Network = LogManager.GetLogger("Network");


        public static void InitNLog()
        {
            var filePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName);
            var name = EnvUtils.GetOrDefault("NLOG_CONFIG_NAME", "NLog.config");
            filePath = $"{filePath}/{name}";
            LogManager.LogFactory.Setup().LoadConfigurationFromFile(filePath);

            Network.Info("NLog initialized");
        }
    }
}
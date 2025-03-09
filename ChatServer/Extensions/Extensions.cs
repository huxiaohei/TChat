/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 15:54:31
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using NLog;
using Network;
using Utils.Log;
using Utils.Envs;
using System.Diagnostics;
using Orleans.Configuration;
using ChatServer.Grains;
using NLog.Extensions.Logging;
using Abstractions.Grains;
using Abstractions.Network;
using Abstractions.Message;
using Microsoft.Extensions.Logging.Configuration;


namespace ChatServer.Extensions
{

    public static class Extensions
    {
        public static void AddDefaultServices(this ServerBuilder builder)
        {
        }

        public static void AddNLog(this ServerBuilder builder)
        {
            var filePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName);
            var name = EnvUtils.GetOrDefault("NLOG_CONFIG_NAME", "NLog.config");
            filePath = $"{filePath}/{name}";
            LogManager.LogFactory.Setup().LoadConfigurationFromFile(filePath);

            builder.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConfiguration();
                builder.AddNLog();
            });

            Loggers.Chat.Info("AddNLog NLog is ready.");
        }

        public static bool TryListenTcp(this ServerBuilder builder)
        {
            if (EnvUtils.TryGetEnv("TcpPort", out var tcpPort))
            {
                builder.ListenTcp(int.Parse(tcpPort), options =>
                {
                    // options.UseConnectionHandler();
                });
                return true;
            }
            return false;
        }

        public static async Task StartGrainServerAsync(this ServerBuilder builder)
        {
            var siloHostBuild = Host.CreateDefaultBuilder()
                .UseOrleans(silo =>
                {
                    silo.AddGrainService<BaseGrainService>()
                        .ConfigureServices(services =>
                        {
                            services.AddSingleton(builder.App.Services.GetRequiredService<ISessionManager>());
                            services.AddSingleton(builder.App.Services.GetRequiredService<ILoggerFactory>());
                            services.AddSingleton<IBaseGrainServiceClient, BaseGrainServiceClient>();
                        })
                        .UseDashboard();
                    silo.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "dev";
                        options.ServiceId = "TChat";
                    });
                    var siloConnectionAddr = EnvUtils.GetOrDefault("SiloConnection", "");
                    if (!string.IsNullOrEmpty(siloConnectionAddr))
                    {
                    }
                    else
                    {
                        silo.UseLocalhostClustering().ConfigureLogging(logging =>
                        {
                            logging.AddNLog();
                        });
                        silo.UseInMemoryReminderService();
                    }
                });
            var siloHost = siloHostBuild.Build();
            await siloHost.StartAsync();

            var silo = siloHost.Services.GetRequiredService<Silo>();
            var clusterClient = siloHost.Services.GetRequiredService<IClusterClient>();
            var messageHandler = builder.App.Services.GetRequiredService<IMessageHandler>();
            messageHandler.Bind(clusterClient, silo.SiloAddress);
        }

    }

}
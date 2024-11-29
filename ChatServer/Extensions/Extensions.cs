/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 15:54:31
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using TChat.Network;
using TChat.Utils.Envs;
using System.Diagnostics;
using NLog.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using NLog;
using TChat.Utils.Log;
using TChat.ChatServer.Grains;
using TChat.Abstractions.Grains;
using TChat.Abstractions.Network;
using Orleans.Configuration;
using System.Net;

namespace TChat.ChatServer.Extensions
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

            builder.Builder.Services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConfiguration();
                builder.AddNLog();
            });

            Loggers.Network.Info("AddNLog NLog is ready.");
        }

        public static bool TryListenTcp(this ServerBuilder builder)
        {
            if (EnvUtils.TryGetEnv("TcpPort", out var tcpPort))
            {
                builder.Builder.WebHost.ConfigureKestrel((context, kesterl) =>
                {
                    kesterl.Listen(IPAddress.IPv6Any, int.Parse(tcpPort), options =>
                    {
                        // options.UseConnectionHandler<TcpConnectionHandler>();
                    });
                });
                return true;
            }
            return false;
        }

        public static async Task StartGrainServerAsync(this ServerBuilder builder)
        {
            var siloHostBuild = new HostBuilder()
                .UseOrleans(silo =>
                {
                    silo.AddGrainService<BaseGrainService>()
                        .ConfigureServices(services =>
                        {
                            services.AddSingleton(builder.Builder.Services.BuildServiceProvider().GetRequiredService<ISessionManager>());
                            services.AddSingleton(builder.Builder.Services.BuildServiceProvider().GetRequiredService<ILoggerFactory>());
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
                        silo.UseLocalhostClustering();
                        silo.UseInMemoryReminderService();
                    }
                });
            var siloHost = siloHostBuild.Build();
            await siloHost.StartAsync();

            var silo = siloHost.Services.GetRequiredService<Silo>();
            var clusterClient = siloHost.Services.GetRequiredService<IClusterClient>();
            builder.Builder.Services.AddSingleton(silo.SiloAddress);
            builder.Builder.Services.AddSingleton(clusterClient);
        }

    }

}
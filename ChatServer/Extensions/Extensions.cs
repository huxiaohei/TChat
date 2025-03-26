/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/28 15:54:31
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using NLog;
using Network;
using Utils.LoggerUtil;
using Utils.EnvUtil;
using System.Diagnostics;
using Orleans.Configuration;
using ChatServer.Grains;
using NLog.Extensions.Logging;
using Abstractions.Grains;
using Abstractions.Network;
using Abstractions.Message;
using Microsoft.Extensions.Logging.Configuration;
using Network.Session;
using ChatServer.Message;

namespace ChatServer.Extensions
{
    public static class Extensions
    {

        public static void AddNLog(this ServerBuilder builder)
        {
            var filePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName);
            var name = Envs.GetOrDefault("NLOG_CONFIG_NAME", "NLog.config");
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
            if (Envs.TryGetEnv("TcpPort", out var tcpPort))
            {
                builder.ListenTcp(int.Parse(tcpPort), options =>
                {
                    // options.UseConnectionHandler();
                });
                return true;
            }
            return false;
        }

        public static async Task StartGrainServerAsync(this ServerBuilder builder, Action<IServiceCollection> services)
        {
            var siloHostBuild = Host.CreateDefaultBuilder()
                .UseOrleans(silo =>
                {
                    silo.AddGrainService<BaseGrainService>()
                        .ConfigureServices(configureDelegate =>
                        {
                            configureDelegate.AddSingleton<ISessionManager, SessionManager>();
                            configureDelegate.AddSingleton<ILoggerFactory, LoggerFactory>();
                            configureDelegate.AddSingleton<IBaseGrainServiceClient, BaseGrainServiceClient>();
                            services(configureDelegate);
                        })
                        .UseDashboard();
                    silo.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "dev";
                        options.ServiceId = "TChat";
                    });
                    var siloConnectionAddr = Envs.GetOrDefault("SiloConnection", "");
                    if (!string.IsNullOrEmpty(siloConnectionAddr))
                    {
                        // TODO
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
            var factory = siloHost.Services.GetRequiredService<IGrainFactory>();
            var messageHandler = new ChatMessageHandler(factory, silo.SiloAddress);

            builder.Services.AddSingleton(factory);
            builder.Services.AddSingleton<IMessageHandler>(messageHandler);
            builder.Services.AddSingleton(siloHost.Services.GetRequiredService<ISessionManager>());
            builder.Services.AddSingleton(siloHost.Services.GetRequiredService<ILoggerFactory>());
        }
    }
}
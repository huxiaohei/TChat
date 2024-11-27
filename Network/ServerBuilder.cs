/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 14:48:20
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using System.Net;
using TChat.Utils.Envs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TChat.Abstractions.Network;
using Tchat.Network.Session;

namespace TChat.Network
{
    public class ServerBuilder
    {
        private WebApplicationBuilder _builder;
        private WebApplication? _app;

        public ServerBuilder()
        {
            _builder = WebApplication.CreateBuilder();
            AddDefaultServices();
        }

        public void AddDefaultServices()
        {
            _builder.Services.AddSingleton<ISessionManager, SessionManager>();
        }

        public void AddSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            _builder.Services.AddSingleton<TService, TImplementation>();
        }

        public bool TryListenTcp()
        {
            if (EnvUtils.TryGetEnv("TcpPort", out var tcpPort))
            {
                _builder.WebHost.ConfigureKestrel((context, kesterl) =>
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

        public void StartGrainServerAsync(bool useWebSockets)
        {
            if (_app != null)
            {
                throw new Exception("Server is already running");
            }
            _builder.Services.AddControllers();
            _app = _builder.Build();
            if (useWebSockets)
            {
                _app.UseWebSockets(new()
                {
                    KeepAliveInterval = TimeSpan.FromMinutes(1)
                }).Use(async (context, next) =>
                {
                    await next(context);
                });
                _app.MapControllers();
            }
            _app.MapControllers();
            _app.Run();
        }

        public void StopServer()
        {
            if (_app == null)
            {
                return;
            }
            _app.StopAsync();
        }
    }
}

/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 14:48:20
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using System.Net;
using Tchat.Network.Session;
using TChat.Abstractions.Network;
using TChat.Abstractions.Message;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace TChat.Network
{
    public class ServerBuilder
    {
        private WebApplicationBuilder _builder;
        public WebApplication? _app { get; private set; }

        public WebApplication App
        {
            get
            {
                if (_app == null)
                {
                    throw new Exception("Server is not built");
                }
                return _app;
            }
            private set
            {
                _app = value;
            }
        }

        public ServerBuilder()
        {
            _builder = WebApplication.CreateBuilder();
            _builder.Services.AddOptions();
            _builder.Services.AddLogging();
            _builder.Services.AddSingleton<ISessionManager, SessionManager>();
        }

        public void AddLogging(Action<ILoggingBuilder> configure)
        {
            _builder.Services.AddLogging(configure);
        }

        public void RegisterMessageHandler<T>() where T : class, IMessageHandler
        {
            _builder.Services.AddSingleton<IMessageHandler, T>();
        }

        public void ListenTcp(int port, Action<ListenOptions> configure)
        {
            _builder.WebHost.ConfigureKestrel((context, kesterl) =>
            {
                kesterl.Listen(IPAddress.IPv6Any, port, configure);
            });
        }

        public void Build()
        {
            _builder.Services.AddControllers();
            App = _builder.Build();
            App.UseWebSockets(new()
            {
                KeepAliveInterval = TimeSpan.FromMinutes(1)
            }).Use(async (context, next) =>
            {
                await next(context);
            });
            App.MapControllers();
        }

        public void Run()
        {
            App.Run();
        }

        public async void Stop()
        {
            await App.StopAsync();
        }
    }
}

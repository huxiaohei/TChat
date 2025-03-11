/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 14:48:20
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Message;
using Abstractions.Network;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Network.Session;
using System.Threading.Channels;
using Utils.Log;

namespace Network
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
                    throw new Exception("Server is not build");
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
                kesterl.ListenAnyIP(port, configure);
            });
        }

        public void AddSwaggerGen()
        {
            _builder.Services.AddSwaggerGen();
        }

        public void Build()
        {
            _builder.Services.AddControllers();
            App = _builder.Build();
            App.UseWebSockets(new()
            {
                KeepAliveInterval = TimeSpan.FromMinutes(1),
            }).Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        Loggers.Network.Info($"Receive a new WebSocket connection from {context.Connection.RemoteIpAddress}");

                        var sessionManager = context.RequestServices.GetRequiredService<ISessionManager>();
                        var handler = context.RequestServices.GetRequiredService<IMessageHandler>();
                        var session = new WebSocketSession(sessionManager, handler, webSocket);
                        sessionManager.AddSession(session);
                        var channel = Channel.CreateBounded<ICSMessage>(20);
                        await Task.WhenAll(session.ReceiveMessageAsync(channel.Writer), session.ProcessMessageAsync(channel.Reader));
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Expected WebSocket request");
                    }
                }
                else
                {
                    await next(context);
                }
            });
        }

        public void UseSwagger()
        {
            App.UseRouting();
            App.UseSwagger();
            App.UseSwaggerUI();
        }

        public void Run()
        {
            App.MapControllers();
            App.Run();
        }

        public async void Stop()
        {
            await App.StopAsync();
        }
    }
}
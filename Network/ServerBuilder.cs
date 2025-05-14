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
using Utils.EnvUtil;
using Utils.LoggerUtil;

namespace Network
{
    public class ServerBuilder
    {
        private readonly WebApplicationBuilder _builder;
        private WebApplication? App { get; set; }

        public WebApplication Application
        {
            get
            {
                if (App == null)
                {
                    throw new Exception("Server is not build");
                }
                return App;
            }
            private set
            {
                App = value;
            }
        }

        public IServiceCollection Services => _builder.Services;

        public ServerBuilder()
        {
            _builder = WebApplication.CreateBuilder();
            _builder.Services.AddOptions();
            _builder.Services.AddLogging();
        }

        public void AddLogging(Action<ILoggingBuilder> configure)
        {
            _builder.Services.AddLogging(configure);
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
            Application = _builder.Build();
            Application.UseWebSockets(new()
            {
                KeepAliveInterval = TimeSpan.FromMinutes(1),
            }).Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        Loggers.Network.Info($"Receive a new WebSocket connection from address:{context.Connection.RemoteIpAddress}");

                        var sessionManager = context.RequestServices.GetRequiredService<ISessionManager>();
                        var handler = context.RequestServices.GetRequiredService<IMessageHandler>();
                        var session = new WebSocketSession(sessionManager, handler, webSocket);
                        sessionManager.AddSession(session);
                        var networkChannelCapacity = Envs.GetOrDefault("NetworkChannelCapacity", 20);
                        var channel = Channel.CreateBounded<ICSMessage>(networkChannelCapacity);
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
            Application.UseRouting();
            Application.UseSwagger();
            Application.UseSwaggerUI();
        }

        public void Run()
        {
            Application.MapControllers();
            Application.Run();
        }

        public async void Stop()
        {
            await Application.StopAsync();
        }
    }
}
/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 14:48:20
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Tchat.Network.Session;
using TChat.Abstractions.Network;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TChat.Network
{
    public class ServerBuilder
    {
        public WebApplicationBuilder Builder { get; }
        public WebApplication? App { get; private set; }

        public ServerBuilder()
        {
            Builder = WebApplication.CreateBuilder();
            Builder.Services.AddOptions();
            Builder.Services.AddLogging();
            Builder.Services.AddSingleton<ISessionManager, SessionManager>();
        }

        public void Run()
        {
            if (App != null)
            {
                throw new Exception("Server is already running");
            }
            Builder.Services.AddControllers();
            App = Builder.Build();
            App.UseWebSockets(new()
            {
                KeepAliveInterval = TimeSpan.FromMinutes(1)
            }).Use(async (context, next) =>
            {
                await next(context);
            });
            App.MapControllers();
            App.Run();
        }

        public void Stop()
        {
            if (App == null)
            {
                return;
            }
            App.StopAsync();
        }
    }
}

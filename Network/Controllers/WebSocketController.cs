/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 11:17:09
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using TChat.Utils.Log;
using TChat.Network.Session;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;
using TChat.Abstractions.Message;
using TChat.Abstractions.Network;
using Microsoft.Extensions.DependencyInjection;

namespace TChat.Network.Controllers
{
    public class WebSocketController(IServiceProvider serviceProvider) : ControllerBase
    {
        [Route("/ws")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                Loggers.Network.Info($"Receive a new WebSocket connection from {HttpContext.Connection.RemoteIpAddress}");

                var sessionManager = serviceProvider.GetRequiredService<ISessionManager>();
                var handler = serviceProvider.GetRequiredService<IMessageHandler>();
                var session = new WebSocketSession(sessionManager, handler, webSocket);
                sessionManager.AddSession(session);
                var channel = Channel.CreateBounded<ICSMessage>(20);
                await Task.WhenAll(session.ReceiveMessageAsync(channel.Writer), session.ProcessMessageAsync(channel.Reader));
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

    }
}
/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 11:17:09
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using Abstractions.Message;
using Abstractions.Network;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Network.Session;
using System.Threading.Channels;
using Utils.Log;

namespace Network.Controllers
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
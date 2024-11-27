/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/11/23 11:17:09
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using TChat.Utils.Log;
using TChat.Network.Session;
using Microsoft.AspNetCore.Mvc;
using TChat.Abstractions.Network;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Channels;
using TChat.Network.Message;

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
                var session = new WebSocketSession(sessionManager, webSocket);
                sessionManager.AddSession(session);
                var channel = Channel.CreateBounded<CSMessage>(20);
                await Task.WhenAll(session.ReceiveMessageAsync(channel.Writer), session.ProcessMessageAsync(channel.Reader));
            }
            else
            {
                HttpContext.Response.StatusCode = 400;
            }
        }

    }
}
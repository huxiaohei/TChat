/*****************************************************************
 * Description 
 * Email huxiaoheigame@gmail.com
 * Created on 2024/12/1 13:35:17
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using TChat.Network;
using TChat.ChatServer.Message;
using TChat.ChatServer.Extensions;

namespace ChatServer
{

    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new ServerBuilder();

            server.AddDefaultServices();

            server.AddNLog();

            server.TryListenTcp();

            server.RegisterMessageHandler<ChatMessageHandler>();

            server.Build();

            await server.StartGrainServerAsync();

            server.Run();
        }
    }
}
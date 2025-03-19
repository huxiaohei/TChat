/*****************************************************************
 * Description
 * Email huxiaoheigame@gmail.com
 * Created on 2024/12/1 13:35:17
 * Copyright (c) 2023 虎小黑
 ****************************************************************/

using ChatServer.Extensions;
using Network;

namespace ChatServer
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var server = new ServerBuilder();

            server.AddNLog();

            server.TryListenTcp();

            server.AddSwaggerGen();

            await server.StartGrainServerAsync(services => { });

            server.Build();

            server.UseSwagger();

            server.Run();
        }
    }
}
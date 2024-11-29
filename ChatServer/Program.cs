
using TChat.ChatServer.Extensions;
using TChat.ChatServer.Message;
using TChat.Network;

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

            // await server.StartGrainServerAsync();

            server.RegisterMessageHandler<ChatMessageHandler>();

            server.Run();
        }
    }
}
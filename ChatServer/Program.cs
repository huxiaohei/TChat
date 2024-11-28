
using TChat.ChatServer.Extensions;
using TChat.ChatServer.Message;
using TChat.Network;

namespace ChatServer
{

    class Program
    {
        static void Main(string[] args)
        {
            var server = new ServerBuilder();

            server.AddDefaultServices();

            server.AddNLog();

            server.TryListenTcp();

            server.StartGrainServerAsync();

            server.RegisterMessageHandler<ChatMessageHandler>();

            server.Run();
        }
    }
}
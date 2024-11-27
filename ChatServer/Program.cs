using NLog;
using TChat.Network;
using TChat.Utils.Log;

namespace ChatServer
{

    class Program
    {
        static void Main(string[] args)
        {
            Loggers.InitNLog();

            var server = new ServerBuilder();
            server.TryListenTcp();
            server.StartGrainServerAsync(true);
        }
    }
}
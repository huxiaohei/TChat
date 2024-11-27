
using TChat.Utils.Log;
using TChat.Network.Protos;
using System.Net.WebSockets;
using TChat.Network.Message;

namespace TChat.Tests.Login
{
    public class LoginTest
    {
        [Fact]
        public async void TestLogin()
        {
            var req = new CSMessage(10001, 1, 1, new CSLoginReq()
            {
                Uid = 10001,
                ServerId = 1001,
                Token = "1234567890abcdef==",
            });
            var client = new ClientWebSocket();
            await client.ConnectAsync(new Uri("ws://localhost:5064/ws"), CancellationToken.None);
            await client.SendAsync(req.Encode(), WebSocketMessageType.Binary, true, CancellationToken.None);

            using var buffer = new RentBuffer(1024 * 8);
            while (client.State == WebSocketState.Open)
            {
                var result = await client.ReceiveAsync(buffer.Buffer, CancellationToken.None);
                if (result.EndOfMessage)
                {
                    var msg = SCMessage.Decode(buffer.Buffer[..result.Count].ToArray());
                    Loggers.Test.Info($"Receive message from Server, MsgName:{msg.MsgName} ClientSerialId:{msg.ClientSerialId} ServerSerialId:{msg.ServerSerialId} Message:{msg.Message}");
                }
            }
        }
    }
}


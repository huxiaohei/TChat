using Network.Message;
using Network.Protos;
using System.Net.WebSockets;
using Utils.LoggerUtil;
using Utils.TimeUtil;

namespace Tests.Login
{
    public class LoginTest
    {
        [Fact]
        public async Task TestLogin()
        {
            uint clientSerialId = 0;
            uint serverSerialId = 0;
            var req = new CSMessage(10001, ++clientSerialId, ++serverSerialId, new CSLoginReq()
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
                    var msg = SCMessage.Decode([.. buffer.Buffer[..result.Count]]);
                    Loggers.Test.Info($"Receive message from Server, MsgName:{msg.MsgName} ClientSerialId:{msg.ClientSerialId} ServerSerialId:{msg.ServerSerialId} Message:{msg.Message}");

                    var ping = new CSMessage(1001, ++clientSerialId, ++serverSerialId, new CSPing() { ClientTimeMs = Time.NowMilliseconds() });
                    await client.SendAsync(ping.Encode(), WebSocketMessageType.Binary, true, CancellationToken.None);

                    await Task.Delay(1000);
                }
            }
        }
    }
}